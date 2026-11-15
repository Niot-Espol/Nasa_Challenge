import React from 'react';
import { Composition, Folder } from 'remotion';
import { MarsCraftsTrailer } from './MarsCraftsTrailer';
import { Scene01_UniverseAwaits } from './scenes/Scene01_UniverseAwaits';
import { Scene02_MissionBegins } from './scenes/Scene02_MissionBegins';
import { Scene03_DesignYourMission } from './scenes/Scene03_DesignYourMission';
import { Scene04_TheLaunch } from './scenes/Scene04_TheLaunch';
import { Scene05_ExploreMars } from './scenes/Scene05_ExploreMars';
import { Scene06_MoonSecret } from './scenes/Scene06_MoonSecret';
import { Scene07_ControlRescue } from './scenes/Scene07_ControlRescue';
import { Scene08_GrandFinale } from './scenes/Scene08_GrandFinale';
import { ensureTrailerFonts } from './styles/theme';

export const RemotionRoot: React.FC = () => {
  ensureTrailerFonts();

  return (
    <>
      {/* Composición Principal Oficial: Exactamente 45 segundos (1350 frames @ 30 FPS, 1920x1080) */}
      <Composition
        id="MarsCraftsTrailer"
        component={MarsCraftsTrailer}
        durationInFrames={1350}
        fps={30}
        width={1920}
        height={1080}
      />

      {/* Previsualización independiente de cada una de las 8 escenas en Remotion Studio */}
      <Folder name="Escenas-Individuales">
        <Composition
          id="Escena01-Universo"
          component={Scene01_UniverseAwaits}
          durationInFrames={120}
          fps={30}
          width={1920}
          height={1080}
        />
        <Composition
          id="Escena02-HubBriefing"
          component={Scene02_MissionBegins}
          durationInFrames={150}
          fps={30}
          width={1920}
          height={1080}
        />
        <Composition
          id="Escena03-DisenaTaller"
          component={Scene03_DesignYourMission}
          durationInFrames={180}
          fps={30}
          width={1920}
          height={1080}
        />
        <Composition
          id="Escena04-Lanzamiento"
          component={Scene04_TheLaunch}
          durationInFrames={210}
          fps={30}
          width={1920}
          height={1080}
        />
        <Composition
          id="Escena05-ExploraMarte"
          component={Scene05_ExploreMars}
          durationInFrames={180}
          fps={30}
          width={1920}
          height={1080}
        />
        <Composition
          id="Escena06-DescubreLuna"
          component={Scene06_MoonSecret}
          durationInFrames={150}
          fps={30}
          width={1920}
          height={1080}
        />
        <Composition
          id="Escena07-ResuelveRescate"
          component={Scene07_ControlRescue}
          durationInFrames={210}
          fps={30}
          width={1920}
          height={1080}
        />
        <Composition
          id="Escena08-CierreLogo"
          component={Scene08_GrandFinale}
          durationInFrames={150}
          fps={30}
          width={1920}
          height={1080}
        />
      </Folder>
    </>
  );
};
