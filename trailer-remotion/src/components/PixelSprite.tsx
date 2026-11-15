import React from "react";
import { Img, staticFile } from "remotion";

export interface PixelSpriteProps {
  path?: string;
  src?: string;
  width?: number;
  height?: number;
  size?: number;
  scale?: number;
  flipX?: boolean;
  style?: React.CSSProperties;
}

export const PixelSprite: React.FC<PixelSpriteProps> = ({
  path,
  src,
  width,
  height,
  size,
  scale = 1,
  flipX = false,
  style,
}) => {
  const resolved = path || src || "";
  const w = (width ?? size ?? 64) * scale;
  const h = (height ?? size ?? width ?? 64) * scale;
  return (
    <Img
      src={staticFile(resolved)}
      style={{
        width: w,
        height: h,
        objectFit: "contain",
        imageRendering: "pixelated",
        transform: flipX ? "scaleX(-1)" : undefined,
        ...style,
      }}
    />
  );
};
