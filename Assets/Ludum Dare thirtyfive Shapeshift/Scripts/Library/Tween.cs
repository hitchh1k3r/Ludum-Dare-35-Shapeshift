using UnityEngine;
using System.Collections;

public static class Tween
{

  // Constants for easy ENUM access:
  public const TransformEaseProperty TRANSPROP_POSITION_GLOBAL = TransformEaseProperty.POSITION_GLOBAL;
  public const TransformEaseProperty TRANSPROP_POSITION = TransformEaseProperty.POSITION;
  public const TransformEaseProperty TRANSPROP_ROTATION = TransformEaseProperty.ROTATION;
  public const TransformEaseProperty TRANSPROP_SCALE = TransformEaseProperty.SCALE;

  public const UIEaseProperty UIPROP_COLOR = UIEaseProperty.COLOR;

  public const EaseType EASE_LINE = EaseType.LINEAR;
  public const EaseType EASE_QUAD = EaseType.QUADRADIC;
  public const EaseType EASE_CUBE = EaseType.CUBIC;
  public const EaseType EASE_SINE = EaseType.SINUSOIDAL;
  public const EaseType EASE_ELASTIC = EaseType.ELASTIC;
  public const EaseType EASE_BOUNCE = EaseType.BOUNCE;
  public const EaseType EASE_NONE = EaseType.NONE;

  public enum TransformEaseProperty
  {
    POSITION_GLOBAL,
    POSITION,
    ROTATION,
    SCALE
  }

  public enum UIEaseProperty
  {
    COLOR
  }

  public enum EaseType
  {
    LINEAR,
    QUADRADIC,
    CUBIC,
    SINUSOIDAL,
    ELASTIC,
    BOUNCE,
    NONE
  }

  public delegate void CallBack();

  public static IEnumerator EaseCoroutine(Transform source, TransformEaseProperty property, object target, float easeTime,
      EaseType easeIn = EaseType.LINEAR, EaseType easeOut = EaseType.LINEAR, CallBack callback = null)
  {
    // Save the original value (to tween from it!), and convert
    // a target transform to the correct property (target can be
    // either the target value or a transform with the value)
    object orig = null;
    switch (property)
    {
      case TransformEaseProperty.POSITION:
        {
          if (target is Transform)
          {
            target = ((Transform)target).localPosition;
          }
          if (target is Vector3)
          {
            orig = source.localPosition;
          }
          break;
        }
      case TransformEaseProperty.POSITION_GLOBAL:
        {
          if (target is Transform)
          {
            target = ((Transform)target).position;
          }
          if (target is Vector3)
          {
            orig = source.position;
          }
          break;
        }
      case TransformEaseProperty.ROTATION:
        {
          if (target is Transform)
          {
            target = ((Transform)target).localRotation;
          }
          if (target is Quaternion)
          {
            orig = source.localRotation;
          }
          break;
        }
      case TransformEaseProperty.SCALE:
        {
          if (target is Transform)
          {
            target = ((Transform)target).localScale;
          }
          if (target is Vector3)
          {
            orig = source.localScale;
          }
          break;
        }
    }

    // orig is only set (above) if the target value was the correct type
    if (orig == null)
    {
      // Error if the target was an incompatable type
      Debug.LogError("Easing with incorrect parameters");
    }
    else
    {
      // Start the easing...
      float timeScale = 1.0f / easeTime;
      float time = 0;
      while (time < 1)
      {
        // Wait until the next frame
        yield return null;

        // Advance iterator and solve easing for this frame
        time += Time.deltaTime * timeScale;
        switch (property)
        {
          case TransformEaseProperty.POSITION:
            {
              source.localPosition = VecEaseInOut((Vector3)orig, (Vector3)target, time, easeIn, easeOut);
              break;
            }
          case TransformEaseProperty.POSITION_GLOBAL:
            {
              source.position = VecEaseInOut((Vector3)orig, (Vector3)target, time, easeIn, easeOut);
              break;
            }
          case TransformEaseProperty.ROTATION:
            {
              Vector3 a = ((Quaternion)orig).eulerAngles;
              Vector3 b = ((Quaternion)target).eulerAngles;
              while (Mathf.Abs(a.x - b.x) > 180)
              {
                if (a.x - b.x < 0)
                {
                  b.x -= 360;
                }
                else
                {
                  b.x += 360;
                }
              }
              while (Mathf.Abs(a.y - b.y) > 180)
              {
                if (a.y - b.y < 0)
                {
                  b.y -= 360;
                }
                else
                {
                  b.y += 360;
                }
              }
              while (Mathf.Abs(a.z - b.z) > 180)
              {
                if (a.z - b.z < 0)
                {
                  b.z -= 360;
                }
                else
                {
                  b.z += 360;
                }
              }
              source.localRotation = Quaternion.Euler(VecEaseInOut(a, b, time, easeIn, easeOut));
              break;
            }
          case TransformEaseProperty.SCALE:
            {
              source.localScale = VecEaseInOut((Vector3)orig, (Vector3)target, time, easeIn, easeOut);
              break;
            }
        }
      }

      // Once the time has elapsed set the property to the final value
      switch (property)
      {
        case TransformEaseProperty.POSITION:
          {
            source.localPosition = (Vector3)target;
            break;
          }
        case TransformEaseProperty.POSITION_GLOBAL:
          {
            source.position = (Vector3)target;
            break;
          }
        case TransformEaseProperty.ROTATION:
          {
            source.localRotation = (Quaternion)target;
            break;
          }
        case TransformEaseProperty.SCALE:
          {
            source.localScale = (Vector3)target;
            break;
          }
      }
    }

    // If we got a callback method, call it
    if (callback != null)
    {
      callback.Invoke();
    }

    // End the coroutine!
    yield break;
  }

  public static IEnumerator EaseCoroutine(UnityEngine.UI.Graphic source, UIEaseProperty property, object target, float easeTime,
      EaseType easeIn = EaseType.LINEAR, EaseType easeOut = EaseType.LINEAR, CallBack callback = null)
  {
    // Save the original value (to tween from it!), and convert
    // a target transform to the correct property (target can be
    // either the target value or a transform with the value)
    object orig = null;
    switch (property)
    {
      case UIEaseProperty.COLOR:
        {
          if (target is Color)
          {
            orig = source.color;
          }
          break;
        }
    }

    // orig is only set (above) if the target value was the correct type
    if (orig == null)
    {
      // Error if the target was an incompatable type
      Debug.LogError("Easing with incorrect parameters");
    }
    else
    {
      // Start the easing...
      float timeScale = 1.0f / easeTime;
      float time = 0;
      while (time < 1)
      {
        // Wait until the next frame
        yield return null;

        // Advance iterator and solve easing for this frame
        time += Time.deltaTime * timeScale;
        switch (property)
        {
          case UIEaseProperty.COLOR:
            {
              source.color = ColorEaseInOut((Color)orig, (Color)target, time, easeIn, easeOut);
              break;
            }
        }
      }

      // Once the time has elapsed set the property to the final value
      switch (property)
      {
        case UIEaseProperty.COLOR:
          {
            source.color = (Color)target;
            break;
          }
      }
    }

    // If we got a callback method, call it
    if (callback != null)
    {
      callback.Invoke();
    }

    // End the coroutine!
    yield break;
  }

  public static Vector3 VecEaseInOut(Vector3 a, Vector3 b, float time, EaseType inType, EaseType outType)
  {
    // Interpolates vector a to b (time is 0..1) with in and out easing methods
    return a + (EaseToLerp(time, inType, outType) * (b - a));
  }

  public static Color ColorEaseInOut(Color a, Color b, float time, EaseType inType, EaseType outType)
  {
    // Interpolates color a to b (time is 0..1) with in and out easing methods
    return a + (EaseToLerp(time, inType, outType) * (b - a));
  }

  public static float EaseToLerp(float time, EaseType easeIn = EaseType.LINEAR, EaseType easeOut = EaseType.LINEAR)
  {
    // Converts time (0..1) to a interpolation value (0..1) based on in and out easing methods

    // If in or out are EASE_NONE only the other easing is used
    if (easeIn == EaseType.NONE)
    {
      // Ease out...
      return easeOut == EaseType.NONE ? 1 : EaseHelper(time, easeOut);
    }
    if (easeOut == EaseType.NONE)
    {
      // Ease in...
      return 1.0f - EaseHelper(1.0f - time, easeIn);
    }
    // Ease in/out...
    if (time < 0.5f)
    {
      // Half in
      return (0.5f - (EaseHelper((0.5f - time) * 2.0f, easeIn) * 0.5f));
    }
    // Half out
    return (EaseHelper((time - 0.5f) * 2.0f, easeOut) * 0.5f) + 0.5f;
  }

  private static float EaseHelper(float t, EaseType type)
  {
    // Various ease out methods...
    // t (0..1) is time and returns the iterpolation (0..1) for the specified ease out function
    switch (type)
    {
      case EaseType.LINEAR:
        {
          return t;
        }
      case EaseType.QUADRADIC:
        {
          return -t * (t - 2);
        }
      case EaseType.CUBIC:
        {
          t--;
          return (t * t * t) + 1;
        }
      case EaseType.SINUSOIDAL:
        {
          return Mathf.Sin(t * Mathf.PI * 0.5f);
        }
      case EaseType.ELASTIC:
        {
          if (t <= 0)
            return 0;
          if (t >= 1)
            return 1;
          return 1 - Mathf.Pow(2, -10 * t) * Mathf.Sin((t - 0.1909859313f) * (2 * Mathf.PI) / 0.3f);
        }
      case EaseType.BOUNCE:
        {
          if (t < 0.363636f)
          {
            return t * t * 7.5625f;
          }
          if (t < 0.727272f)
          {
            t -= 0.545454f;
            return t * t * 7.5625f + 0.75f;
          }
          if (t < 0.909090f)
          {
            t -= 0.818181f;
            return t * t * 7.5625f + 0.9375f;
          }
          t -= 0.954545f;
          return t * t * 7.5625f + 0.984375f;
        }
      case EaseType.NONE:
        {
          return 0.5f;
        }
    }
    return t;
  }

  // TODO (hitch) maybe make a coroutine method for this too?
  //     oh... also test this method, ^_^ I've never used it!
  public static Vector3 Bezier(Vector3 aV, Vector3 bV, Vector3 cV, float t)
  {
    // Interpolates aV to cV by t (0..1) with a smooth slope tangentially passing through aVbV and bVcV
    float aS = (1 - t) * (1 - t);
    float bS;
    float cS = t * t;
    bS = 1 - aS - cS;
    return (aS * aV) + (bS * bV) + (cS * cV);
  }

}