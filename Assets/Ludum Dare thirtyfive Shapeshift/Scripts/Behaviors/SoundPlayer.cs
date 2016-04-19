using UnityEngine;

public class SoundPlayer : MonoBehaviour
{

  public AudioSource rotate, slide, stop, interact, death, smash, jump, bigJump, split, combine;

  private static SoundPlayer instance;

  void Awake()
  {
    instance = this;
  }

  public static void PlaySound(Sound sound, float volume)
  {
    AudioSource clip = null;
    switch (sound)
    {
      case Sound.BLOCK_ROTATE:
        {
          clip = instance.rotate;
          break;
        }
      case Sound.BLOCK_SLIDE:
        {
          clip = instance.slide;
          break;
        }
      case Sound.BLOCK_STOP:
        {
          clip = instance.stop;
          break;
        }
      case Sound.INTERACT:
        {
          clip = instance.interact;
          break;
        }
      case Sound.DEATH:
        {
          clip = instance.death;
          break;
        }
      case Sound.SMASH_SHIP:
        {
          clip = instance.smash;
          break;
        }
      case Sound.JUMP_SMALL:
        {
          clip = instance.jump;
          break;
        }
      case Sound.JUMP_BIG:
        {
          clip = instance.bigJump;
          break;
        }
      case Sound.SPLIT:
        {
          clip = instance.split;
          break;
        }
      case Sound.COMBINE:
        {
          clip = instance.combine;
          break;
        }
    }
    if (clip != null)
    {
      clip.volume = volume;
      clip.Play();
    }
  }

  public enum Sound
  {
    BLOCK_ROTATE,
    BLOCK_SLIDE,
    BLOCK_STOP,
    INTERACT,
    DEATH,
    SMASH_SHIP,
    JUMP_SMALL,
    JUMP_BIG,
    SPLIT,
    COMBINE
  }

}
