import React from 'react';
import { interpolate, spring, useCurrentFrame, useVideoConfig } from 'remotion';
import { COLORS, FONTS } from '../styles/theme';

interface KineticKeywordCardProps {
  keyword: string;
  subtitle: string;
  accentColor?: string;
  align?: 'left' | 'right';
}

export const KineticKeywordCard: React.FC<KineticKeywordCardProps> = ({
  keyword,
  subtitle,
  accentColor = COLORS.cyanTech,
  align = 'left',
}) => {
  const frame = useCurrentFrame();
  const { fps, durationInFrames } = useVideoConfig();

  const enter = spring({
    frame,
    fps,
    config: { damping: 14, stiffness: 180, mass: 0.6 },
  });

  const exit = interpolate(
    frame,
    [durationInFrames - 12, durationInFrames],
    [1, 0],
    { extrapolateLeft: 'clamp', extrapolateRight: 'clamp' }
  );

  return (
    <div
      style={{
        position: 'absolute',
        top: 88,
        left: align === 'left' ? 76 : undefined,
        right: align === 'right' ? 76 : undefined,
        backgroundColor: 'rgba(11, 14, 22, 0.88)',
        borderLeft: align === 'left' ? `6px solid ${accentColor}` : `2px solid ${accentColor}55`,
        borderRight: align === 'right' ? `6px solid ${accentColor}` : `2px solid ${accentColor}55`,
        borderTop: `2px solid ${accentColor}55`,
        borderBottom: `2px solid ${accentColor}55`,
        padding: '14px 28px',
        boxShadow: `0 12px 32px rgba(0, 0, 0, 0.7), 0 0 24px ${accentColor}33`,
        transform: `translateX(${(1 - enter) * (align === 'left' ? -60 : 60)}px)`,
        opacity: enter * exit,
        textAlign: align,
      }}
    >
      <div
        style={{
          fontFamily: FONTS.pixel,
          fontSize: 54,
          color: accentColor,
          letterSpacing: 4,
          lineHeight: 1.0,
          textShadow: `0 0 18px ${accentColor}88, 0 4px 0 #05070C`,
        }}
      >
        {keyword}
      </div>
      <div
        style={{
          fontFamily: FONTS.sans,
          fontSize: 20,
          fontWeight: 800,
          color: '#EEF0F3',
          letterSpacing: 1.2,
          marginTop: 6,
          textTransform: 'uppercase',
        }}
      >
        {subtitle}
      </div>
    </div>
  );
};
