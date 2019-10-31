﻿namespace Dungeon.Monogame
{
    using Dungeon.Audio;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;
    using Microsoft.Xna.Framework.Media;
    using System.Collections.Generic;

    public partial class XNADrawClient : Game, IAudioPlayer
    {
        public void Music(string name, AudioOptions audioOptions = null)
        {
            //var song = LoadSong(name);
            //MediaPlayer.Stop();
            //MediaPlayer.Play(song);
            //MediaPlayer.IsRepeating = audioOptions?.Repeat ?? false;
            //MediaPlayer.Volume = (float)(audioOptions?.Volume ?? .5);
        }

        public void Effect(string effect, AudioOptions audioOptions = null)
        {
            var sound = LoadSound(effect).CreateInstance();
            sound.Volume = (float)(audioOptions?.Volume ?? .1);
            sound.Play();
        }

        private readonly Dictionary<string, SoundEffect> soundEffectsCache = new Dictionary<string, SoundEffect>();

        private SoundEffect LoadSound(string name)
        {
            if(!soundEffectsCache.TryGetValue(name,out var sound))
            {
                sound = Content.Load<SoundEffect>($@"Audio\Sound\{name}");
                soundEffectsCache[name] = sound;
            }

            return sound;
        }

        /// <summary>
        /// Надеемся что внутри контента есть кэширование
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private Song LoadSong(string name) => Content.Load<Song>($@"Audio\Music\{name}");
    }
}