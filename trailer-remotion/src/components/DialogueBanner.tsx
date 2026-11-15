import React from 'react';
import { Img, interpolate, spring, staticFile, useCurrentFrame, useVideoConfig } from 'remotion';
import { COLORS, FONTS } from '../styles/theme';

interface DialogueBannerProps {
  speaker: string;
  roleLabel?: string;
  portraitPath: string;
  text: string;
  accentColor?: string;
  bottomOffset?: number;
}

export const DialogueBanner: React.FC<DialogueBannerProps> = ({
  speaker,
  roleLabel,
  portraitPath,
  text,
  accentColor = COLORS.cyanTech,
  bottomOffset = 82,
}) => {
  const frame = useCurrentFrame();
  const { fps } = useVideoConfig();

  const enter = spring({
    frame,
    fps,
    config: { damping: 16, stiffness: 160, mass: 0.7 },
  });

  const charsVisible = Math.floor(
    interpolate(frame, [4, 4 + Math.max(15, text.length * 0.65)], [0, text.length], {
      extrapolateLeft: 'clamp',
      extrapolateRight: 'clamp',
    })
  );

  return (
    <div
      style={{
        position: 'absolute',
        left: 200,
        right: 200,
        bottom: bottomOffset,
        height: 164,
        backgroundColor: 'rgba(14, 19, 32, 0.94)',
        border: `3px solid ${accentColor}`,
        boxShadow: `0 14px 36px rgba(0, 0, 0, 0.75), 0 0 22px ${accentColor}33`,
        borderRadius: 10,
        display: 'flex',
        alignItems: 'center',
        padding: '14px 24px',
        gap: 24,
        transform: `translateY(${(1 - enter) * 36}px)`,
        opacity: enter,
      }}
    >
      <div
        style={{
          width: 128,
          height: 128,
          backgroundColor: '#0A0E17',
          border: `3px solid ${accentColor}`,
          borderRadius: 6,
          flexShrink: 0,
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
          overflow: 'hidden',
        }}
      >
        <Img
          src={staticFile(portraitPath)}
          className="pixel-art"
          style={{
            width: 120,
            height: 120,
            imageRendering: 'pixelated',
          }}
        />
      </div>

      <div style={{ flex: 1, display: 'flex', flexDirection: 'column', gap: 8 }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
          <span
            style={{
              fontFamily: FONTS.pixel,
              fontSize: 26,
              color: accentColor,
              letterSpacing: 1.2,
              textTransform: 'uppercase',
            }}
          >
            {speaker}
          </span>
          {roleLabel && (
            <span
              style={{
                fontFamily: FONTS.sans,
                fontSize: 16,
                fontWeight: 700,
                color: '#C4C8CF',
                backgroundColor: 'rgba(255,255,255,0.08)',
                padding: '3px 10px',
                borderRadius: 4,
              }}
            >
              {roleLabel}
            </span>
          )}
        </div>

        <div
          style={{
            fontFamily: FONTS.sans,
            fontSize: 25,
            fontWeight: 700,
            color: '#EEF0F3',
            lineHeight: 1.32,
          }}
        >
          «{text.slice(0, charsVisible)}»
        </div>
      </div>
    </div>
  );
};
