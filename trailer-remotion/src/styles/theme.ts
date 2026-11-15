import { staticFile } from 'remotion';

export const COLORS = {
  spaceVoid: '#06080E',
  spaceDeep: '#0B0E16',
  spacePanel: '#141926',
  spaceBorder: '#2A3550',
  cyanTech: '#38E8FF',
  cyanGlow: '#8FE3F7',
  goldFoil: '#FFB300',
  amberAlert: '#FF7A2F',
  crimsonDanger: '#FF3B47',
  emeraldOk: '#38E596',

  // Paleta Marte (Jezero)
  marsShadow: '#3B1712',
  marsDark: '#782C1E',
  marsMid: '#B94A2C',
  marsLight: '#E07A47',
  marsHighlight: '#F4B87A',

  // Paleta Luna (Polo Sur)
  moonVoid: '#0B0E16',
  moonDark: '#262B36',
  moonSlate: '#4E5562',
  moonBase: '#858B96',
  moonLight: '#C4C8CF',
  moonPeak: '#EEF0F3',
  iceCore: '#3FA3D6',
  iceGlow: '#E8FBFF',
};

export const FONTS = {
  title: "'PixelifySans', monospace",
  body: "'Nunito', sans-serif",
  pixel: 'PixelifySansTrailer',
  sans: 'NunitoTrailer',
};

let fontsInjected = false;

export const ensureTrailerFonts = () => {
  if (typeof document === 'undefined' || fontsInjected) return;
  fontsInjected = true;
  const style = document.createElement('style');
  style.innerHTML = `
    @font-face {
      font-family: 'PixelifySansTrailer';
      src: url('${staticFile('game-assets/Fonts/PixelifySans.ttf')}') format('truetype');
      font-weight: 700;
      font-style: normal;
    }
    @font-face {
      font-family: 'NunitoTrailer';
      src: url('${staticFile('game-assets/Fonts/Nunito.ttf')}') format('truetype');
      font-weight: 400 800;
      font-style: normal;
    }
    .pixel-art {
      image-rendering: pixelated;
      image-rendering: crisp-edges;
    }
  `;
  document.head.appendChild(style);
};
