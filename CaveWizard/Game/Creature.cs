using System;
using System.Collections.Generic;
using CaveEngine.ScreenSystem;
using CaveWizard.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CaveWizard.Game
{
    public enum CreatureState
    {
        Moving,
        Idle,
        Attack,
        Jump, 
        Dead
    }
    public abstract class Creature: TexturedWorldObject
    {
        protected CreatureState CreatureState;
        protected TimeSpan AttackTimeStamp;
        protected Dictionary<CreatureState, TimeSpan> AnimationTimeSpans;
        protected int Health;
        protected Creature(ScreenManager screenManager, string propName, Vector2 objectBodySize, Vector2 objectTextureMetersSize, int columns, int rows, Level sourceLevel) : base(screenManager, propName, objectBodySize, objectTextureMetersSize, columns, rows, sourceLevel)
        {
            CreatureState = CreatureState.Idle;
        }
      
    }
}