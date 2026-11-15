import React from 'react';
import { AbsoluteFill, interpolate, random, useCurrentFrame, useVideoConfig } from 'remotion';

interface CinematicCameraProps {
  children: React.ReactNode;
  zoomStart?: number;
  zoomEnd?: number;
  panXStart?: number;
  panXEnd?: number;
  panYStart?: number;
  panYEnd?: number;
  shakeIntensity?: number;
  shakeSeed?: string;
  letterboxHeight?: number;
  vignetteOpacity?: number;
  tintColor?: string;
  overlayNode?: React.ReactNode;
}

export const CinematicCamera: React.FC<CinematicCameraProps> = ({
  children,
  zoomStart = 1,
  zoomEnd = 1.06,
  panXStart = 0,
  panXEnd = 0,
  panYStart = 0,
  panYEnd = 0,
  shakeIntensity = 0,
  shakeSeed = 'cam',
  letterboxHeight = 64,
  vignetteOpacity = 0.45,
  tintColor,
  overlayNode,
}) => {
  const frame = useCurrentFrame();
  const { durationInFrames } = useVideoConfig();

  const zoom = interpolate(frame, [0, durationInFrames], [zoomStart, zoomEnd], {
    extrapolateLeft: 'clamp',
    extrapolateRight: 'clamp',
  });
  const panX = interpolate(frame, [0, durationInFrames], [panXStart, panXEnd], {
    extrapolateLeft: 'clamp',
    extrapolateRight: 'clamp',
  });
  const panY = interpolate(frame, [0, durationInFrames], [panYStart, panYEnd], {
    extrapolateLeft: 'clamp',
    extrapolateRight: 'clamp',
  });

  const shakeX =
    shakeIntensity > 0
      ? (random(`${shakeSeed}-x-${frame}`) - 0.5) * 2 * shakeIntensity
      : 0;
  const shakeY =
    shakeIntensity > 0
      ? (random(`${shakeSeed}-y-${frame}`) - 0.5) * 2 * shakeIntensity
      : 0;

  const fadeIn = interpolate(frame, [0, 8], [0, 1], {
    extrapolateLeft: 'clamp',
    extrapolateRight: 'clamp',
  });
  const fadeOut = interpolate(
    frame,
    [durationInFrames - 8, durationInFrames],
    [1, 0],
    { extrapolateLeft: 'clamp', extrapolateRight: 'clamp' }
  );

  return (
    <AbsoluteFill style={{ backgroundColor: '#06080E', overflow: 'hidden' }}>
      <AbsoluteFill
        style={{
          transform: `translate3d(${(panX + shakeX).toFixed(1)}px, ${(panY + shakeY).toFixed(1)}px, 0) scale(${zoom.toFixed(4)})`,
          transformOrigin: 'center center',
        }}
      >
        {children}
      </AbsoluteFill>

      {tintColor && (
        <AbsoluteFill
          style={{
            background: tintColor,
            pointerEvents: 'none',
          }}
        />
      )}

      {vignetteOpacity > 0 && (
        <AbsoluteFill
          style={{
            background: `radial-gradient(circle at 50% 50%, rgba(0,0,0,0) 52%, rgba(2,4,10,${vignetteOpacity}) 100%)`,
            pointerEvents: 'none',
          }}
        />
      )}

      {/* Letterbox cinematográfico 2.35:1 */}
      {letterboxHeight > 0 && (
        <>
          <div
            style={{
              position: 'absolute',
              top: 0,
              left: 0,
              right: 0,
              height: letterboxHeight,
              backgroundColor: '#04060B',
              borderBottom: '1px solid rgba(56, 232, 255, 0.18)',
              zIndex: 40,
            }}
          />
          <div
            style={{
              position: 'absolute',
              bottom: 0,
              left: 0,
              right: 0,
              height: letterboxHeight,
              backgroundColor: '#04060B',
              borderTop: '1px solid rgba(56, 232, 255, 0.18)',
              zIndex: 40,
            }}
          />
        </>
      )}

      {/* UI / Textos encima de la cámara */}
      {overlayNode && (
        <AbsoluteFill style={{ zIndex: 50, pointerEvents: 'none' }}>
          {overlayNode}
        </AbsoluteFill>
      )}

      {/* Suavizado de corte en bordes de secuencia */}
      <AbsoluteFill
        style={{
          backgroundColor: '#04060B',
          opacity: 1 - Math.min(fadeIn, fadeOut),
          pointerEvents: 'none',
          zIndex: 60,
        }}
      />
    </AbsoluteFill>
  );
};
