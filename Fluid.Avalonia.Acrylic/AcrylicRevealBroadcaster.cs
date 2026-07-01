using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Input;
using global::Avalonia.Interactivity;
using global::Avalonia.VisualTree;

namespace Fluid.Avalonia.Acrylic
{
    /// <summary>
    /// One shared <see cref="TopLevel"/> pointer subscription per window, fanning out to every
    /// reveal-enabled <see cref="AcrylicSurface"/> under it — mirrors <see cref="AcrylicBackdropProvider"/>'s
    /// per-TopLevel subscriber pattern so N reveal surfaces don't each hook their own pointer handlers.
    /// Surfaces that never enable <see cref="AcrylicSurface.RevealBorderEnabled"/> are never registered,
    /// so they pay zero overhead.
    /// </summary>
    internal static class AcrylicRevealBroadcaster
    {
        private sealed class BroadcastState
        {
            public List<WeakReference<AcrylicSurface>> Surfaces { get; } = new();
            public bool Hooked;
        }

        private static readonly ConditionalWeakTable<TopLevel, BroadcastState> s_states = new();

        public static void Register(AcrylicSurface surface)
        {
            TopLevel? topLevel = TopLevel.GetTopLevel(surface);
            if (topLevel is null)
                return;

            BroadcastState state = s_states.GetOrCreateValue(topLevel);
            CleanupDeadRefs(state);

            foreach (WeakReference<AcrylicSurface> existing in state.Surfaces)
            {
                if (existing.TryGetTarget(out AcrylicSurface? target) && ReferenceEquals(target, surface))
                {
                    EnsureHooked(topLevel, state);
                    return;
                }
            }

            state.Surfaces.Add(new WeakReference<AcrylicSurface>(surface));
            EnsureHooked(topLevel, state);
        }

        public static void Unregister(TopLevel? topLevel, AcrylicSurface surface)
        {
            if (topLevel is null || !s_states.TryGetValue(topLevel, out BroadcastState? state))
                return;

            for (int i = state.Surfaces.Count - 1; i >= 0; i--)
            {
                if (!state.Surfaces[i].TryGetTarget(out AcrylicSurface? existing) || ReferenceEquals(existing, surface))
                    state.Surfaces.RemoveAt(i);
            }

            if (state.Surfaces.Count == 0)
                Unhook(topLevel, state);
        }

        private static void EnsureHooked(TopLevel topLevel, BroadcastState state)
        {
            if (state.Hooked)
                return;

            topLevel.AddHandler(InputElement.PointerMovedEvent, OnPointerMoved, RoutingStrategies.Tunnel, true);
            topLevel.AddHandler(InputElement.PointerExitedEvent, OnPointerExited, RoutingStrategies.Tunnel, true);
            state.Hooked = true;
        }

        private static void Unhook(TopLevel topLevel, BroadcastState state)
        {
            if (!state.Hooked)
                return;

            topLevel.RemoveHandler(InputElement.PointerMovedEvent, OnPointerMoved);
            topLevel.RemoveHandler(InputElement.PointerExitedEvent, OnPointerExited);
            state.Hooked = false;
        }

        private static void OnPointerMoved(object? sender, PointerEventArgs e)
        {
            if (sender is not TopLevel topLevel || !s_states.TryGetValue(topLevel, out BroadcastState? state))
                return;

            Broadcast(state, topLevel, e.GetPosition(topLevel));
        }

        private static void OnPointerExited(object? sender, PointerEventArgs e)
        {
            if (sender is not TopLevel topLevel || !s_states.TryGetValue(topLevel, out BroadcastState? state))
                return;

            Broadcast(state, topLevel, null);
        }

        private static void Broadcast(BroadcastState state, TopLevel topLevel, Point? topLevelPosition)
        {
            CleanupDeadRefs(state);

            foreach (WeakReference<AcrylicSurface> weakSurface in state.Surfaces)
            {
                if (!weakSurface.TryGetTarget(out AcrylicSurface? surface) || !surface.IsAttachedToVisualTree())
                    continue;

                if (topLevelPosition is null)
                {
                    surface.SetRevealPointer(default, 0.0);
                    continue;
                }

                // TransformToVisual(target) maps points from the source's local space into
                // target's local space — here from the TopLevel directly into each surface's own
                // coordinates, so no manual matrix inversion is needed.
                Matrix? transform = topLevel.TransformToVisual(surface);
                if (transform is null)
                {
                    surface.SetRevealPointer(default, 0.0);
                    continue;
                }

                Point local = topLevelPosition.Value.Transform(transform.Value);
                double proximity = Math.Max(0.0, surface.RevealProximityDistance);
                double distance = DistanceToRect(local, surface.Bounds.Size);

                double intensity = proximity <= 0.0001
                    ? (distance <= 0.0 ? 1.0 : 0.0)
                    : Clamp(1.0 - distance / proximity, 0.0, 1.0);

                surface.SetRevealPointer(local, intensity);
            }
        }

        private static double DistanceToRect(Point p, Size size)
        {
            double dx = Math.Max(0.0, Math.Max(-p.X, p.X - size.Width));
            double dy = Math.Max(0.0, Math.Max(-p.Y, p.Y - size.Height));
            return Math.Sqrt(dx * dx + dy * dy);
        }

        private static void CleanupDeadRefs(BroadcastState state)
        {
            for (int i = state.Surfaces.Count - 1; i >= 0; i--)
            {
                if (!state.Surfaces[i].TryGetTarget(out _))
                    state.Surfaces.RemoveAt(i);
            }
        }

        private static double Clamp(double value, double min, double max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
    }
}
