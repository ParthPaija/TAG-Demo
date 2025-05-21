using DG.Tweening;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public static class ExtensionClass
{
    public static Vector3 Clamp(this Vector3 value, Vector3 min, Vector3 max)
    {
        value.x = Mathf.Clamp(value.x, min.x, max.x);
        value.y = Mathf.Clamp(value.y, min.y, max.y);
        value.z = Mathf.Clamp(value.z, min.z, max.z);
        return value;
    }

    public static void ScrollToRect(this ScrollRect scrollRect, Vector3 viewTransformPosition, bool playAnim = false, float animationTime = 0.3f)
    {
        var targetPosition = scrollRect.GetTargetPositionScrollToRect(viewTransformPosition);

        // Animate the content to the target position
        if (playAnim)
            scrollRect.content.DOAnchorPos(targetPosition, animationTime).SetEase(Ease.OutQuad);
        else
            scrollRect.content.anchoredPosition = targetPosition;
    }

    public static Vector2 GetTargetPositionScrollToRect(this ScrollRect scrollRect, Vector3 viewTransformPosition)
    {
        int direction = scrollRect.horizontal ? -1 : 1;
        int widthMultiPlier = scrollRect.horizontal ? 1 : 0;
        int heightMultiPlier = scrollRect.vertical ? 1 : 0;

        RectTransform contentRect = scrollRect.content;
        RectTransform viewPortRect = scrollRect.viewport;

        // Calculate the position of the view transform relative to the content
        Vector2 viewPosition = contentRect.InverseTransformPoint(viewTransformPosition);

        // Calculate the center of the viewport
        Vector2 viewportCenter = new Vector2(viewPortRect.rect.width * 0.5f * widthMultiPlier, viewPortRect.rect.height * 0.5f * heightMultiPlier);

        // Calculate the target position of the content
        Vector2 targetPosition = -(viewPosition + viewportCenter * direction);

        // Clamp the target position to ensure the content doesn't scroll beyond its bounds
        float maxPos = (contentRect.rect.width - viewPortRect.rect.width) * widthMultiPlier + (contentRect.rect.height - viewPortRect.rect.height) * heightMultiPlier;
        float minVal = direction < 0f ? -maxPos : 0f;
        float maxVal = direction < 0f ? 0f : maxPos;

        targetPosition.x = Mathf.Clamp(targetPosition.x, minVal, maxVal) * widthMultiPlier;
        targetPosition.y = Mathf.Clamp(targetPosition.y, minVal, maxVal) * heightMultiPlier;

        return targetPosition;
    }

    public static float GetAnimationLength(this Animator animator, string clipName)
    {
        RuntimeAnimatorController cont = animator.runtimeAnimatorController;
        for (int i = 0; i < cont.animationClips.Length; i++)
        {
            if (cont.animationClips[i].name == clipName)
                return cont.animationClips[i].length;
        }
        return 0;
    }

    public static string ParseTimeSpan(this TimeSpan timeSpanToCovert, int maxParams = 3)
    {
        string currentFormat = "{0:hh}h {0:mm}m {0:ss}s";
        if (maxParams == 2)
        {
            if (timeSpanToCovert.TotalDays >= 1)
                currentFormat = "{0:%d}d {0:hh}h";
            else if (timeSpanToCovert.Hours >= 1)
                currentFormat = "{0:hh}h {0:mm}m";
            else
                currentFormat = "{0:mm}m {0:ss}s";
        }
        else if (maxParams == 3 && timeSpanToCovert.TotalDays >= 1)
        {
            currentFormat = "{0:%d}d {0:hh}h {0:mm}m";
        }

        return string.Format(currentFormat, timeSpanToCovert);
    }

    public static T GetConverted<T>(this Dictionary<string, object> dic, string key, T defaultValue)
    {
        if (dic == null)
            return defaultValue;

        if (dic.ContainsKey(key) && dic[key] != null)
        {
            return (T)Convert.ChangeType(dic[key], typeof(T));
        }

        return defaultValue;
    }

    public static T GetJObjectCast<T>(this Dictionary<string, object> dic, string key, T defaultValue)
    {
        if (dic == null)
            return defaultValue;
        if (dic.ContainsKey(key) && dic[key] != null)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(dic[key].ToString());
            }
            catch (Exception)
            {
                return (T)Convert.ChangeType(dic[key], typeof(T));
            }
        }

        return defaultValue;
    }

    public static T GetJObjectCast<T>(this object dic)
    {
        try
        {
            return JsonConvert.DeserializeObject<T>(dic.ToString());
        }
        catch (Exception)
        {
            return (T)Convert.ChangeType(dic, typeof(T));
        }
    }

    public static bool ContainsItem(this Dictionary<string, object> dic, string key)
    {
        return dic != null && dic.ContainsKey(key);
    }

    public static int GetGemsCount(this TimeSpan timeSpan)
    {
        if (timeSpan.TotalMinutes <= 60)
            return Mathf.CeilToInt((float)(timeSpan.TotalSeconds * 0.01067f));
        timeSpan = timeSpan.Subtract(new TimeSpan(1, 0, 0));
        return 39 + Mathf.CeilToInt((float)(timeSpan.TotalSeconds * 0.0072f));
    }

    public static string GetEventTitle(this string notificationIntentData)
    {
        return Regex.Replace(notificationIntentData, "[^a-zA-Z0-9_. ]+", "", RegexOptions.Compiled);
    }

    public static int GetUTF8ByteSize(this string s)
    {
        return Encoding.UTF8.GetByteCount(s);
    }
}