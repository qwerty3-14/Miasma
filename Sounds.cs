using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miasma
{
    public static class Sounds
    {
        public static List<SoundPack> allSounds = new List<SoundPack>();
        public static byte sfxVolume = 50;
        public static byte musicVolume = 50;
        public static Song[] songs = new Song[4];
        public static SoundPack pewpew;
        public static SoundPack beam;
        public static SoundPack shield;
        public static SoundPack zap;
        public static SoundPack artillary;
        public static SoundPack beamLighting;
        public static SoundPack kaboom;
        public static SoundPack menuChange;
        public static SoundPack click;
        public static SoundPack infect;
        public static SoundPack carierLaunch;
        public static SoundPack launchMisc;
        public static void LoadSounds(ContentManager Content)
        {
            songs[0] = Content.Load<Song>("menuTheme");
            songs[1] = Content.Load<Song>("miasmaMix");
            songs[2] = Content.Load<Song>("bossBattle");
            UpdateMusicVolume();
            MediaPlayer.IsRepeating = true;
            PlayMusic(0);

            pewpew = new SoundPack(Content.Load<SoundEffect>("pew"), "pewpew", .4f);
            beam = new SoundPack(Content.Load<SoundEffect>("Beam"), "beam", .5f);
            shield = new SoundPack(Content.Load<SoundEffect>("ShieldBlock"), "shield", .8f);
            zap = new SoundPack(Content.Load<SoundEffect>("EnergyBlast"), "zap", .4f);
            artillary = new SoundPack(Content.Load<SoundEffect>("Zap"), "artillary", .5f);
            beamLighting = new SoundPack(Content.Load<SoundEffect>("BeamLightning"), "lighting", 1f);
            kaboom = new SoundPack(Content.Load<SoundEffect>("kaboom"), "kaboom", 1f);
            menuChange = new SoundPack(Content.Load<SoundEffect>("menuChange"), "menu change", .4f);
            click = new SoundPack(Content.Load<SoundEffect>("click"), "click", 1f);
            infect = new SoundPack(Content.Load<SoundEffect>("infect"), "infect", 1f);
            carierLaunch = new SoundPack(Content.Load<SoundEffect>("carrierLaunch"), "carrier launch", 1f);
            launchMisc = new SoundPack(Content.Load<SoundEffect>("launchMisc"), "launch misc.", 1f);
        }
        public static void UpdateMusicVolume()
        {
            MediaPlayer.Volume = .4f * (float)musicVolume / 50f;
        }
        public static void PlayMusic(int i)
        {
            MediaPlayer.Play(songs[i]);
        }
    }
    public class SoundPack
    {
        readonly SoundEffect sfx;
        readonly float volume;
        TimeSpan lastPlayed;
        public readonly String name;
        public SoundPack(SoundEffect sfx, String name, float volume)
        {
            this.name = name;
            this.sfx = sfx;
            this.volume = volume;
            Sounds.allSounds.Add(this);
        }

        public void Play()
        {
            sfx.Play(volume * (Sounds.sfxVolume*.01f), 1f, 1f);
            lastPlayed = Miasma.elapsedTime;

        }
        public void PlayContinuous()
        {
            if(Miasma.elapsedTime>= lastPlayed + sfx.Duration)
            {
                Play();
            }
        }
    }
}
