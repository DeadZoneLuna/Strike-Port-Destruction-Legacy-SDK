#if UNITY_EDITOR || RUNTIME_CSG

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Sabresaurus.SabreCSG.Importers.ValveMapFormat2006
{
    /// <summary>
    /// Represents a Hammer Entity.
    /// </summary>
    public class VmfEntity
    {
        public int Id = -1;

        /// <summary>
        /// The class name of the entity.
        /// </summary>
        public string ClassName;

        /// <summary>
        /// The origin of the entity.
        /// </summary>
        public UnityEngine.Vector3 Origin;

        /// <summary>
        /// The angles of the entity.
        /// </summary>
        public UnityEngine.Vector3 Angles;

        /// <summary>
        /// The pitch (up / down) of the entity.
        /// </summary>
        public string Pitch;

        /// <summary>
        /// The solids in the entity if available.
        /// </summary>
        public List<VmfSolid> Solids = new List<VmfSolid>();

        /// <summary>
        /// <param name="Value">Get Vector from stirng</param>
        /// </summary>
        public UnityEngine.Vector4 GetVector(string Value, bool Reverse = false)
        {
            float[] results = new float[4];
            if (!string.IsNullOrEmpty(Value))
            {
                string[] nums = Value.Split(' ');
                for (int i = 0; i < results.Length && i < nums.Length; ++i)
                {
                    try
                    {
                        results[i] = float.Parse(nums[i], CultureInfo.CreateSpecificCulture("en-US"));
                    }
                    catch
                    {
                        results[i] = 0;
                    }
                }
            }

            //Source Vector:
            //X +forward/-backward 0
            //Y +left/-right 1
            //Z +up/-down 2
            //Unity Vector:
            //X +left/-right 1
            //Y +up/-down 2
            //Z +forward/-backward 0
            //Source QAngle:
            //X pitch +down/-up 0
            //Y yaw +left/-right 1
            //Z roll +right/-left 2
            //Unity Angle:
            //X pitch +down/-up 0
            //Y yaw +left/-right 1
            //Z roll +right/-left 2
            if (Reverse)
                return new UnityEngine.Vector4(-results[0], results[2], -results[1], results[3]);//return new UnityEngine.Vector4(results[1], results[2], results[0], results[3]);
            else
                return new UnityEngine.Vector4(results[0], results[1], results[2], results[3]);
        }

        public void SetupLightNormalFromProps(UnityEngine.Quaternion angles, float angle, float pitch, ref UnityEngine.Vector3 output)
        {
            if (angle == -1)
            {
                output[0] = output[1] = 0;
                output[2] = 1;
            }
            else if (angle == -2)
            {
                output[0] = output[1] = 0;
                output[2] = -1;
            }
            else
            {
                // if we don't have a specific "angle" use the "angles" YAW
                if (angle == 0F)
                {
                    angle = angles[1];
                }

                output[2] = 0;
                output[0] = (float)Math.Cos(angle / 180 * Math.PI);
                output[1] = (float)Math.Sin(angle / 180 * Math.PI);
            }

            if (pitch == 0F)
            {
                // if we don't have a specific "pitch" use the "angles" PITCH
                pitch = angles[0];
            }

            output[2] = (float)Math.Sin(pitch / 180 * Math.PI);
            output[0] *= (float)Math.Cos(pitch / 180 * Math.PI);
            output[1] *= (float)Math.Cos(pitch / 180 * Math.PI);
        }

        /// <summary>
        /// Gets a numeric attribute as a <c>float</c>. Throws if the attribute could not be converted to a numerical value
        /// and no <paramref name="failDefault"/> was provided.
        /// </summary>
        /// <param name="key">Name of the attribute to retrieve.</param>
        /// <param name="failDefault">Value to return if <paramref name="key"/> doesn't exist, or couldn't be converted.</param>
        /// <returns>The numeric value of the value corresponding to <paramref name="key"/>.</returns>
        public float GetFloat(string key, float? failDefault = null)
        {
            try
            {
                return float.Parse(key, CultureInfo.CreateSpecificCulture("en-US"));
            }
            catch (Exception e)
            {
                if (!failDefault.HasValue)
                {
                    throw e;
                }
                return failDefault.Value;
            }
        }

        public float RAD2DEG(double x)
        {
            return (float)x * (float)(180.0f / Math.PI);
        }

        //-----------------------------------------------------------------------------
        // Forward direction vector -> Euler angles
        //-----------------------------------------------------------------------------

        public void VectorAngles(UnityEngine.Vector3 forward, ref UnityEngine.Quaternion angles)
        {
            float tmp, yaw, pitch;

            if (forward[1] == 0 && forward[0] == 0)
            {
                yaw = 0;
                if (forward[2] > 0)
                    pitch = 270;
                else
                    pitch = 90;
            }
            else
            {
                yaw = (UnityEngine.Mathf.Atan2(forward[1], forward[0]) * 180 / UnityEngine.Mathf.PI);
                if (yaw < 0)
                    yaw += 360;

                tmp = UnityEngine.Mathf.Sqrt(forward[0] * forward[0] + forward[1] * forward[1]);
                pitch = (UnityEngine.Mathf.Atan2(-forward[2], tmp) * 180 / UnityEngine.Mathf.PI);
                if (pitch < 0)
                    pitch += 360;
            }

            angles[0] = pitch;
            angles[1] = yaw;
            angles[2] = 0;
        }

        //-----------------------------------------------------------------------------
        // Forward direction vector with a reference up vector -> Euler angles
        //-----------------------------------------------------------------------------

        public void VectorAngles(UnityEngine.Vector3 forward, UnityEngine.Vector3 pseudoup, ref UnityEngine.Quaternion angles)
        {

            UnityEngine.Vector3 left;

            left = UnityEngine.Vector3.Cross(pseudoup, forward);//CrossProduct(pseudoup, forward, left);
            left = UnityEngine.Vector3.Normalize(left);//VectorNormalizeFast(left);

            float xyDist = UnityEngine.Mathf.Sqrt(forward[0] * forward[0] + forward[1] * forward[1]);//sqrtf(forward[0] * forward[0] + forward[1] * forward[1]);

            // enough here to get angles?
            if (xyDist > 0.001f)
            {
                // (yaw)	y = ATAN( forward.y, forward.x );		-- in our space, forward is the X axis
                angles[1] = RAD2DEG(UnityEngine.Mathf.Atan2(forward[1], forward[0]));

                // The engine does pitch inverted from this, but we always end up negating it in the DLL
                // UNDONE: Fix the engine to make it consistent
                // (pitch)	x = ATAN( -forward.z, sqrt(forward.x*forward.x+forward.y*forward.y) );
                angles[0] = RAD2DEG(UnityEngine.Mathf.Atan2(-forward[2], xyDist));

                float up_z = (left[1] * forward[0]) - (left[0] * forward[1]);

                // (roll)	z = ATAN( left.z, up.z );
                angles[2] = RAD2DEG(UnityEngine.Mathf.Atan2(left[2], up_z));
            }
            else    // forward is mostly Z, gimbal lock-
            {
                // (yaw)	y = ATAN( -left.x, left.y );			-- forward is mostly z, so use right for yaw
                angles[1] = RAD2DEG(UnityEngine.Mathf.Atan2(-left[0], left[1])); //This was originally copied from the "void MatrixAngles( const matrix3x4_t& matrix, float *angles )" code, and it's 180 degrees off, negated the values and it all works now (Dave Kircher)

                // The engine does pitch inverted from this, but we always end up negating it in the DLL
                // UNDONE: Fix the engine to make it consistent
                // (pitch)	x = ATAN( -forward.z, sqrt(forward.x*forward.x+forward.y*forward.y) );
                angles[0] = RAD2DEG(UnityEngine.Mathf.Atan2(-forward[2], xyDist));

                // Assume no roll in this case as one degree of freedom has been lost (i.e. yaw == roll)
                angles[2] = 0;
            }
        }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
    public override string ToString()
        {
            return "VmfEntity " + ClassName + " " + Id + " (" + Solids.Count + " Solids)";
        }
    }
}

#endif