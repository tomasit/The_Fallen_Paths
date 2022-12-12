using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundData
{
    public enum SoundEffectName
    {
        UI_CHARACTER_POP,
        INTERACTION_LOCK,
        INTERACTION_DOOR_OPEN,
        INTERACTION_LEVER_IN,
        INTERACTION_LEVER_OUT,
        INTERACTION_BREAK_METAL,
        INTERACTION_FIRE_IN,
        INTERACTION_FIRE_OUT,
        PLAYER_WALK,
        PLAYER_LAND,
        PLAYER_JUMP,
        INTERACTION_CHEST
    }

    public enum SoundEffectType : int
    {
        MUSIC = 1,
        PLAYER = 2,
        ENVIRONMENT = 3,
        UI = 4
    }

    private static Dictionary<SoundEffectName, AudioClip> _soundEffect = null;

    public static Dictionary<SoundEffectName, AudioClip> SoundEffect {
        get {
            if (_soundEffect == null)
            {
                InstantiateSoundEffect();
            }
            return _soundEffect;
        }
    }

    private static void InstantiateSoundEffect()
    {
        _soundEffect = new Dictionary<SoundEffectName, AudioClip>();
        _soundEffect.Add(SoundEffectName.UI_CHARACTER_POP, Resources.Load<AudioClip>("SoundDesign/UI/UI_Beep_Single_Clean_Dark_stereo"));
        _soundEffect.Add(SoundEffectName.INTERACTION_LOCK, Resources.Load<AudioClip>("SoundDesign/Interaction/UI_Error_Double_Note_Down_Dark_stereo"));
        _soundEffect.Add(SoundEffectName.INTERACTION_DOOR_OPEN, Resources.Load<AudioClip>("SoundDesign/Interaction/DOOR_Wood_Bedroom_Open_02_mono"));
        _soundEffect.Add(SoundEffectName.INTERACTION_BREAK_METAL, Resources.Load<AudioClip>("SoundDesign/Interaction/DEMOLISH_Metal_Quick_stereo"));
        _soundEffect.Add(SoundEffectName.INTERACTION_LEVER_IN, Resources.Load<AudioClip>("SoundDesign/Interaction/CREAK_Wood_Tree_Creak_05_Subtle_mono"));
        _soundEffect.Add(SoundEffectName.INTERACTION_LEVER_OUT, Resources.Load<AudioClip>("SoundDesign/Interaction/CREAK_Wood_Tree_Creak_04_Quick_mono"));
        _soundEffect.Add(SoundEffectName.INTERACTION_FIRE_IN, Resources.Load<AudioClip>("SoundDesign/Interaction/MAGIC_SPELL_Flame_02_mono"));
        _soundEffect.Add(SoundEffectName.INTERACTION_FIRE_OUT, Resources.Load<AudioClip>("SoundDesign/Interaction/170235_roachpowder_stove-water-sizzle"));
        _soundEffect.Add(SoundEffectName.PLAYER_WALK, Resources.Load<AudioClip>("SoundDesign/Player/FOOTSTEP_Trainers_Asphalt_Run_RR2_mono"));
        _soundEffect.Add(SoundEffectName.PLAYER_LAND, Resources.Load<AudioClip>("SoundDesign/Player/FOOTSTEP_Leather_Wood_Hollow_Land_RR6_mono"));
        _soundEffect.Add(SoundEffectName.PLAYER_JUMP, Resources.Load<AudioClip>("SoundDesign/Player/FOOTSTEP_Trainers_Asphalt_Run_RR8_mono"));
        _soundEffect.Add(SoundEffectName.INTERACTION_CHEST, Resources.Load<AudioClip>("SoundDesign/Interaction/KEY_Out_Wood_Door_01_mono"));
    }
}