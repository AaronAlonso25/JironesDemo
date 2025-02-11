using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

namespace NeonBlood
{
    /// <summary>
    /// 
    /// </summary>
    public static class ProgrammerTools
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="endValue"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static IEnumerator ApproachTo(float value, float endValue, float time)
        {
            float t = 0f;
            while (value < endValue)
            {
                t += Time.deltaTime / time;
                value += t;

                yield return 0;
            }
        }

        public static IEnumerator ApproachWeightTo(Volume volume, float endValue, float time)
        {
            float temp = volume.weight;

            float t = 0f;
            while (t < 1.0f)
            {
                volume.weight = Mathf.SmoothStep(temp, endValue, t);
                t += Time.deltaTime / time;

                yield return null;
            }

            volume.weight = endValue;
        }

        public static IEnumerator ApproachPulsarTo(Material pulsar, float endValue, float time)
        {
            float temp = pulsar.GetFloat("_Amplitude");

            float t = 0f;
            while (t < 1.0f)
            {
                pulsar.SetFloat("_Amplitude", Mathf.SmoothStep(temp, endValue, t));
                t += Time.deltaTime / time;

                yield return null;
            }

            pulsar.SetFloat("_Amplitude", endValue);
        }

        /// <summary>
        /// Mueve un objeto del punto A al punto B en el tiempo indicado
        /// </summary>
        /// <param name="movingObject">GameObject</param>
        /// <param name="pointA">Punto A</param>
        /// <param name="pointB">Punto B</param>
        /// <param name="time">Tiempo</param>
        public static IEnumerator MoveFromTo(GameObject movingObject, Vector3 pointA, Vector3 pointB, float time)
        {
            float t = 0f;
            // while (Vector3.Distance(movingObject.transform.position, pointB) > 0.1)
            while (Vector3.Distance(movingObject.transform.position, pointB) > 0.00001)
            {
                t += Time.deltaTime / time;
                movingObject.transform.position = Vector3.Lerp(pointA, pointB, t);

                yield return null;
            }
        }

        /// <summary>
		/// Moves an object from point A to point B in a given time
		/// </summary>
		/// <param name="movingObject">Moving object.</param>
		/// <param name="pointA">Point a.</param>
		/// <param name="pointB">Point b.</param>
		/// <param name="time">Time.</param>
		public static IEnumerator MoveLocalFromTo(GameObject movingObject, Vector3 pointA, Vector3 pointB, float time, float approximationDistance)
        {
            float t = 0f;

            float distance = Vector3.Distance(movingObject.transform.localPosition, pointB);

            while (distance >= approximationDistance)
            {
                distance = Vector3.Distance(movingObject.transform.localPosition, pointB);
                t += Time.deltaTime / time;
                movingObject.transform.localPosition = Vector3.Lerp(pointA, pointB, t);
                yield return 0;
            }
            yield break;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="movingObject"></param>
        /// <param name="target"></param>
        /// <param name="speed_"></param>
        /// <returns></returns>
        public static IEnumerator MoveConstanlyFromTo(GameObject movingObject, Vector3 target, float speed_)
        {
            float speed = speed_ * Time.deltaTime;
            while (movingObject.transform.position != target)
            {
                movingObject.transform.position = Vector3.MoveTowards(movingObject.transform.position, target, speed);
                yield return 0;
            }
        }

        public static IEnumerator MoveConstanlyFromTo(GameObject movingObject, Vector3 target, float speed_, float maxDistanceToDetect)
        {
            float speed = speed_ * Time.deltaTime;
            while (Vector3.Distance(movingObject.transform.position, target) > maxDistanceToDetect)
            {
                movingObject.transform.position = Vector3.MoveTowards(movingObject.transform.position, target, speed);
                yield return 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotatingObject"></param>
        /// <param name="pointA"></param>
        /// <param name="pointB"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static IEnumerator RotateFromTo(GameObject rotatingObject, Vector3 pointA, Vector3 pointB, float time)
        {
            float t = 0f;
            while (t < time)
            {
                t += Time.deltaTime;
                rotatingObject.transform.localEulerAngles = Vector3.Lerp(pointA, pointB, t / time);
                yield return 0;
            }

            rotatingObject.transform.localEulerAngles = pointB;
        }

        public static IEnumerator RotateFromToLocal(GameObject rotatingObject, Vector3 limit, Vector3 speed)
        {
            while (Vector3.SqrMagnitude(rotatingObject.transform.localEulerAngles - limit) > 0.01f)
            {
                rotatingObject.transform.Rotate(speed * Time.deltaTime, Space.Self);
                yield return 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotatingObject"></param>
        /// <param name="byAngles"></param>
        /// <param name="inTime"></param>
        /// <returns></returns>
        public static IEnumerator RotateFromTo(GameObject rotatingObject, Vector3 byAngles, float inTime)
        {
            Quaternion initRot = rotatingObject.transform.rotation;
            Quaternion endRot = Quaternion.Euler(byAngles);

            for (float x = 0; x < 1.0; x += Time.deltaTime / inTime)
            {
                rotatingObject.transform.rotation = Quaternion.Lerp(initRot, endRot, x);
                yield return null;
            }
            rotatingObject.transform.rotation = endRot;
        }

        /// <summary>
        /// Scales an object from point A to point B in a given time
        /// </summary>
        /// <param name="scalingObject">Moving object.</param>
        /// <param name="pointA">Point a.</param>
        /// <param name="pointB">Point b.</param>
        /// <param name="time">Time.</param>
        public static IEnumerator ScaleFromTo(GameObject scalingObject, float pointA, float pointB, float time)
        {
            float t = 0f;
            while (t < 1.0f)
            {
                t += Time.deltaTime / time;
                scalingObject.transform.localScale = Vector3.Lerp(
                    new Vector3(pointA, pointA, pointA),
                    new Vector3(pointB, pointB, pointB),
                    t);

                yield return 0;
            }

            scalingObject.transform.localScale = new Vector3(pointB, pointB, pointB);
        }

        public static Transform FindChild(Transform parent, string childName)
        {
            foreach (Transform child in parent)
            {
                if (child.name == childName)
                    return child;
                else
                {
                    Transform found = FindChild(child, childName);
                    if (found != null)
                        return found;
                }
            }
            return null;
        }

        public static string ReplaceTextButtons(string description, string color)
        {
            string temp = description;
            string platform = "";
            if (OptionsManager.Instance.CurrentPlatform == OptionsManager.Platform.STEAM)
            {
                if (OptionsManager.Instance.CurrentInput == OptionsManager.Input.CONTROLLER)
                    platform = "Xbox" + color + "_";
                else
                    platform = "PC" + color + "_";
            }
            else if (OptionsManager.Instance.CurrentPlatform == OptionsManager.Platform.PS4 ||
                OptionsManager.Instance.CurrentPlatform == OptionsManager.Platform.PS5)
            {
                platform = "Sony" + color + "_";
            }                
            else if(OptionsManager.Instance.CurrentPlatform == OptionsManager.Platform.GAMECORE ||
                OptionsManager.Instance.CurrentPlatform == OptionsManager.Platform.SWITCH)
            {
                platform = "Xbox" + color + "_";
            }

            temp = temp.Replace("btn_cross", "<sprite name=\"" + platform + "1\">");
            temp = temp.Replace("btn_circle", "<sprite name=\"" + platform + "0\">");
            temp = temp.Replace("btn_triangle", "<sprite name=\"" + platform + "3\">");
            temp = temp.Replace("btn_square", "<sprite name=\"" + platform + "2\">");

            temp = temp.Replace("btn_r1", "<sprite name=\"" + platform + "11\">");
            temp = temp.Replace("btn_r2", "<sprite name=\"" + platform + "9\">");
            temp = temp.Replace("btn_l1", "<sprite name=\"" + platform + "10\">");
            temp = temp.Replace("btn_l2", "<sprite name=\"" + platform + "8\">");

            temp = temp.Replace("btn_dUp", "<sprite name=\"" + platform + "6\">");
            temp = temp.Replace("btn_dDown", "<sprite name=\"" + platform + "4\">");
            temp = temp.Replace("btn_dLeft", "<sprite name=\"" + platform + "7\">");
            temp = temp.Replace("btn_dRight", "<sprite name=\"" + platform + "5\">");

            temp = temp.Replace("btn_select", "<sprite name=\"" + platform + "13\">");

            temp = temp.Replace("btn_joystickLeft", "<sprite name=\"" + platform + "16\">");
            temp = temp.Replace("btn_joystickRight", "<sprite name=\"" + platform + "17\">");

            return temp;
        }
    }
}