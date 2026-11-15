import React from "react";
import { AbsoluteFill, interpolate, useCurrentFrame } from "remotion";

export const ImpactFlash: React.FC<{
  triggerFrame: number;
  duration?: number;
  color?: string;
  maxOpacity?: number;
}> = ({ triggerFrame, duration = 8, color = "#ffffff", maxOpacity = 0.75 }) => {
  const frame = useCurrentFrame();
  if (frame < triggerFrame || frame > triggerFrame + duration) return null;
  const opacity = interpolate(
    frame,
    [triggerFrame, triggerFrame + 1, triggerFrame + duration],
    [0, maxOpacity, 0],
    { extrapolateLeft: "clamp", extrapolateRight: "clamp" }
  );
  return (
    <AbsoluteFill
      style={{
        backgroundColor: color,
        opacity,
        mixBlendMode: "screen",
        pointerEvents: "none",
        zIndex: 90,
      }}
    />
  );
};

export const SpeedLinesOverlay: React.FC<{
  opacity?: number;
  color?: string;
  direction?: "vertical" | "horizontal" | "diagonal";
}> = ({ opacity = 0.28, color = "#38bdf8", direction = "vertical" }) => {
  const frame = useCurrentFrame();
  const lines = Array.from({ length: 28 }, (_, i) => {
    const pos = (i * 37 + frame * 19) % 100;
    const len = 18 + ((i * 23) % 45);
    const thick = 2 + (i % 3) * 2;
    const offset = ((frame * (28 + (i % 7) * 8) + i * 130) % 140) - 20;
    return { pos, len, thick, offset };
  });

  const rotation =
    direction === "horizontal" ? 90 : direction === "diagonal" ? -22 : 0;

  return (
    <AbsoluteFill
      style={{
        opacity,
        pointerEvents: "none",
        overflow: "hidden",
        transform: `rotate(${rotation}deg) scale(1.25)`,
        zIndex: 45,
      }}
    >
      {lines.map((l, idx) => (
        <div
          key={idx}
          style={{
            position: "absolute",
            left: `${l.pos}%`,
            top: `${l.offset}%`,
            width: l.thick,
            height: `${l.len}%`,
            background: `linear-gradient(180deg, transparent 0%, ${color} 50%, #ffffff 100%)`,
            boxShadow: `0 0 12px ${color}`,
          }}
        />
      ))}
    </AbsoluteFill>
  );
};

export const ForegroundParticles: React.FC<{
  color?: string;
  secondaryColor?: string;
  colorA?: string;
  colorB?: string;
  seed?: string;
  count?: number;
  speedY?: number;
  speedX?: number;
}> = ({
  color,
  secondaryColor,
  colorA,
  colorB,
  count = 24,
  speedY = -4.5,
  speedX = 2.0,
}) => {
  const frame = useCurrentFrame();
  const particles = Array.from({ length: count }, (_, i) => {
    const seedX = (i * 173) % 1920;
    const seedY = (i * 293) % 1080;
    const size = 6 + (i % 4) * 4;
    const x = (seedX + frame * speedX * (1 + (i % 3) * 0.4) + 3840) % 1920;
    const y = (seedY + frame * speedY * (1 + (i % 4) * 0.35) + 2160) % 1080;
    const c1 = color || colorA || "#f59e0b"; const c2 = secondaryColor || colorB || "#38bdf8"; const c = i % 3 === 0 ? c2 : c1;
    const alpha = 0.38 + ((i * 17) % 50) / 100;
    return { x, y, size, c, alpha };
  });

  return (
    <AbsoluteFill style={{ pointerEvents: "none", zIndex: 50, overflow: "hidden" }}>
      {particles.map((p, idx) => (
        <div
          key={idx}
          style={{
            position: "absolute",
            left: p.x,
            top: p.y,
            width: p.size,
            height: p.size,
            backgroundColor: p.c,
            opacity: p.alpha,
            boxShadow: `0 0 ${p.size * 2}px ${p.c}`,
          }}
        />
      ))}
    </AbsoluteFill>
  );
};

export const TacticalHudFrame: React.FC<{
  missionCode?: string;
  statusText?: string;
  telemetryText?: string;
  accentColor?: string;
  alertMode?: boolean;
}> = ({ alertMode = false }) => {
  const frame = useCurrentFrame();
  const pulse = 0.5 + 0.5 * Math.sin(frame * 0.25);

  return (
    <AbsoluteFill style={{ pointerEvents: "none", zIndex: 80 }}>
      <AbsoluteFill
        style={{
          boxShadow: alertMode
            ? `inset 0 0 ${95 + pulse * 55}px rgba(239,68,68,0.6)`
            : "inset 0 0 80px rgba(2,6,23,0.55)",
        }}
      />
    </AbsoluteFill>
  );
};

export const DiagonalWipeIn: React.FC<{
  duration?: number;
  color?: string;
}> = ({ duration = 12, color = "#38bdf8" }) => {
  const frame = useCurrentFrame();
  if (frame > duration) return null;
  const progress = interpolate(frame, [0, duration], [0, 140], {
    extrapolateLeft: "clamp",
    extrapolateRight: "clamp",
  });
  return (
    <AbsoluteFill style={{ pointerEvents: "none", zIndex: 95, overflow: "hidden" }}>
      <div
        style={{
          position: "absolute",
          top: -300,
          bottom: -300,
          left: `${progress - 35}%`,
          width: "140%",
          background: "#020617",
          transform: "skewX(-18deg)",
          borderLeft: `10px solid ${color}`,
          boxShadow: `-20px 0 50px ${color}`,
        }}
      />
    </AbsoluteFill>
  );
};
