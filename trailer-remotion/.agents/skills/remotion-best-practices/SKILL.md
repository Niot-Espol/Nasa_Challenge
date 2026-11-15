---
name: remotion-best-practices
description: Official Remotion rules and pixel-art animation best practices for deterministic React + TypeScript video rendering at 30/60 FPS.
---

# Remotion Best Practices (Mars Crafts Trailer Edition)

## 1. Determinism & Frame-Driven Animation
- **Always drive animations from `useCurrentFrame()` and `useVideoConfig()`**.
- **Never use `setTimeout`, `setInterval`, CSS `transition`, or `requestAnimationFrame`**, as Remotion renders frames in parallel and out of order.
- Use `interpolate(frame, inputRange, outputRange, { extrapolateLeft: 'clamp', extrapolateRight: 'clamp', easing })` and `spring({ frame, fps, config })` for smooth motion.
- For particle systems, camera shake, or starfields, use a **deterministic pseudo-random generator (`random('seed-' + index)`)** from `remotion` instead of `Math.random()`.

## 2. Pixel Art Integrity (16-Bit Aesthetic)
- Every pixel-art `<Img>` or sprite container MUST enforce:
  ```css
  image-rendering: pixelated;
  image-rendering: crisp-edges;
  ```
- Prefer integer scale factors (`2x`, `3x`, `4x`, `6x`, `8x`) for sprites and round screen coordinates (`Math.round(x)`, `Math.round(y)`) inside pixel scenes to prevent sub-pixel shimmering, while applying smooth cinematic camera pans/zooms at the outer `<CinematicCamera>` viewport level.

## 3. Asset Loading & `<Sequence>` Composition
- Reference all static files in `public/` using `staticFile('path/to/file')` and `<Img>` / `<Audio>` / `<OffthreadVideo>` from `remotion` so Remotion waits for every frame's assets before capturing.
- Break a long trailer into modular `<Sequence from={...} durationInFrames={...} name="...">` blocks AND register each scene as its own `<Composition>` in `Root.tsx` for isolated previewing in Remotion Studio.
