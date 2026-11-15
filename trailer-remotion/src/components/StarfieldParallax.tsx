import React from 'react';
import { AbsoluteFill, random, useCurrentFrame } from 'remotion';

interface StarfieldParallaxProps {
  seed?: string;
  starCount?: number;
  speedX?: number;
  speedY?: number;
  nebulaColorA?: string;
  nebulaColorB?: string;
}

export const StarfieldParallax: React.FC<StarfieldParallaxProps> = ({
  seed = 'mars-crafts-stars',
  starCount = 110,
  speedX = -0.35,
  speedY = 0.08,
  nebulaColorA = 'rgba(28, 78, 122, 0.28)',
  nebulaColorB = 'rgba(185, 74, 44, 0.22)',
}) => {
  const frame = useCurrentFrame();

  const stars = React.useMemo(() => {
    return Array.from({ length: starCount }).map((_, i) => {
      const depth = 1 + (i % 3); // 1=far, 2=mid, 3=near
      const size = depth === 3 ? 6 : depth === 2 ? 4 : 2;
      const x = random(`${seed}-x-${i}`) * 1920;
      const y = random(`${seed}-y-${i}`) * 1080;
      const twinklePhase = random(`${seed}-tw-${i}`) * Math.PI * 2;
      const tint =
        i % 7 === 0 ? '#8FE3F7' : i % 11 === 0 ? '#F4B87A' : '#EEF0F3';
      return { x, y, size, depth, twinklePhase, tint };
    });
  }, [seed, starCount]);

  return (
    <AbsoluteFill
      style={{
        background: `radial-gradient(ellipse at 25% 30%, ${nebulaColorA}, transparent 60%), radial-gradient(ellipse at 78% 72%, ${nebulaColorB}, transparent 65%), #070A12`,
      }}
    >
      {stars.map((s, idx) => {
        const px = ((s.x + frame * speedX * s.depth) % 1920 + 1920) % 1920;
        const py = ((s.y + frame * speedY * s.depth) % 1080 + 1080) % 1080;
        const twinkle =
          0.55 + 0.45 * Math.sin(frame * 0.12 + s.twinklePhase);
        return (
          <div
            key={idx}
            style={{
              position: 'absolute',
              left: Math.round(px),
              top: Math.round(py),
              width: s.size,
              height: s.size,
              backgroundColor: s.tint,
              opacity: twinkle,
              boxShadow:
                s.depth === 3 ? `0 0 8px ${s.tint}` : undefined,
            }}
          />
        );
      })}
    </AbsoluteFill>
  );
};
