using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace XDTGame.Core
{
    public static partial class MathUtility
    {
        private const float Epsilon = 0.0001f;
        private const float kLogNegligibleResidual = -4.605170186f;
        public const float kNegligibleResidual = 0.01f;
        private const float FloatCompareTolerance = 0.000001f;

        /// <summary>
        /// 算偏航角
        /// </summary>
        /// <param name="xz"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalcYaw(this Vector2 xz)
        {
            return Mathf.Atan2(xz.x, xz.y) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// 判断是否在右侧
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsRight(Vector3 from, Vector3 to)
        {
            return Vector3.SignedAngle(from, to, Vector3.up) >= 0;
        }

        public static float DetachedLookAtTargetDamp(float initial, float dampTime, float deltaTime)
        {
            dampTime = Mathf.Lerp(Mathf.Max(1, dampTime), dampTime, 1);
            deltaTime = Mathf.Lerp(0, deltaTime, 1);
            return Damp(initial, dampTime, deltaTime);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Damp(Vector3 initial, Vector3 dampTime, float deltaTime)
        {
            for (int i = 0; i < 3; ++i)
                initial[i] = Damp(initial[i], dampTime[i], deltaTime);
            return initial;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DecayConstant(float time, float residual)
        {
            return Mathf.Log(1f / residual) / time;
        }

        /// <summary>
        /// 获取阻尼值
        /// </summary>
        /// <param name="initial">阻尼量</param>
        /// <param name="dampTime">阻尼率</param>
        /// <param name="deltaTime">阻尼时间</param>
        /// <returns>阻尼粱， 按比例缩放返回个 0-1的值</returns>
        public static float Damp(float initial, float dampTime, float deltaTime)
        {
            if (dampTime < Epsilon || Mathf.Abs(initial) < Epsilon)
                return initial;
            if (deltaTime < Epsilon)
                return 0;
            float k = -kLogNegligibleResidual / dampTime;

            float step = Time.fixedDeltaTime;
            if (deltaTime != step)
                step /= 5;
            int numSteps = Mathf.FloorToInt(deltaTime / step);
            float vel = initial * step / deltaTime;
            float decayConstant = Mathf.Exp(-k * step);
            float r = 0;
            for (int i = 0; i < numSteps; ++i)
                r = (r + vel) * decayConstant;
            float d = deltaTime - (step * numSteps);
            if (d > Epsilon)
                r = Mathf.Lerp(r, (r + vel) * decayConstant, d / step);
            return initial - r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DampAngle(float current, float target, float dampTime, float deltaTime)
        {
            target = current + Mathf.DeltaAngle(current, target);
            return Damp(target - current, dampTime, deltaTime);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ProjectOntoPlane(Vector3 vector, Vector3 planeNormal)
        {
            return (vector - Vector3.Dot(vector, planeNormal) * planeNormal);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Project(in Vector2 vector2,in  Vector2 normal)
        {
            Vector3 vector3 = new Vector3(vector2.x, 0f, vector2.y);
            Vector3 normal3 = new Vector3(normal.x, 0f, normal.y);
            Vector3 project = Vector3.Project(vector3, normal3);
            return new Vector2(project.x, project.z);
        }

        public static Rect ScreenToFOV(Rect rScreen, float fov, float fovH, float aspect)
        {
            Rect r = new Rect(rScreen);
            Matrix4x4 persp = Matrix4x4.Perspective(fov, aspect, 0.0001f, 2f).inverse;

            Vector3 p = persp.MultiplyPoint(new Vector3(0, (r.yMin * 2f) - 1f, 0.5f));
            p.z = -p.z;
            float angle = Vector3.SignedAngle(Vector3.forward, p, Vector3.left);
            r.yMin = ((fov / 2) + angle) / fov;

            p = persp.MultiplyPoint(new Vector3(0, (r.yMax * 2f) - 1f, 0.5f));
            p.z = -p.z;
            angle = Vector3.SignedAngle(Vector3.forward, p, Vector3.left);
            r.yMax = ((fov / 2) + angle) / fov;

            p = persp.MultiplyPoint(new Vector3((r.xMin * 2f) - 1f, 0, 0.5f));
            p.z = -p.z;
            angle = Vector3.SignedAngle(Vector3.forward, p, Vector3.up);
            r.xMin = ((fovH / 2) + angle) / fovH;

            p = persp.MultiplyPoint(new Vector3((r.xMax * 2f) - 1f, 0, 0.5f));
            p.z = -p.z;
            angle = Vector3.SignedAngle(Vector3.forward, p, Vector3.up);
            r.xMax = ((fovH / 2) + angle) / fovH;
            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ScreenPointToCanvas(Vector2 screenPoint, RectTransform rectTransform)
        {
            var rect = rectTransform.rect;
            Vector2 canvasPosition = new Vector2(screenPoint.x / Screen.width * rect.width,
                screenPoint.y / Screen.height * rect.height);
            return canvasPosition;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion CalculateAngle(float axisX, float axisY)
        {
            return Quaternion.Euler(new Vector3(90 - axisY, axisX + 180, 0));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 AngleToAxis(Quaternion angle)
        {
            return AngleToAxis(angle.eulerAngles);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 AngleToAxis(Vector3 angle)
        {
            return new Vector2(angle.y - 180, 90 - angle.x);
        }

        public static Vector3 CalculatePosition(float angleRadiusX, float angleRadiusY, float distance)
        {
            return new Vector3(Mathf.Sin(angleRadiusY) * Mathf.Sin(angleRadiusX) * distance,
                Mathf.Cos(angleRadiusY) * distance,
                Mathf.Sin(angleRadiusY) * Mathf.Cos(angleRadiusX) * distance);
        }

        public static Vector3 CalculatePosition(Quaternion angle, float distance)
        {
            var a = AngleToAxis(angle) * Mathf.Deg2Rad;
            return new Vector3(Mathf.Sin(a.y) * Mathf.Sin(a.x) * distance,
                Mathf.Cos(a.y) * distance,
                Mathf.Sin(a.y) * Mathf.Cos(a.x) * distance);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AlmostZero(this Vector3 v)
        {
            return v.sqrMagnitude < (Epsilon * Epsilon);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AlmostZero(this Vector2 v)
        {
            return v.sqrMagnitude < (Epsilon * Epsilon);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AlmostEqual(this float n, float other, float tolerance = FloatCompareTolerance)
        {
            return Mathf.Abs(n - other) <= tolerance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AlmostEqual(this Vector3 n, Vector3 other, float tolerance = FloatCompareTolerance)
        {
            return Vector3.SqrMagnitude(n - other) <= tolerance;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AlmostEqual(this Vector2 n, Vector2 other, float tolerance = FloatCompareTolerance)
        {
            return Vector2.SqrMagnitude(n - other) <= tolerance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float UniformAngle360(this float angle)
        {
            return Mathf.Repeat(angle, 360f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalcuAngle(this float angle1, float angle2)
        {
            float angle = Mathf.Abs(angle1 - angle2);
            angle = angle.UniformAngle360();
            return (angle > 180) ? angle - 360 : angle;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ClampAngle(this float angle1, float angle2)
        {
            angle1 = angle1.UniformAngle360();
            angle2 = angle2.UniformAngle360();
            float clampAngle = angle2 - angle1;

            if (clampAngle > 180f)
            {
                clampAngle -= 360f;
            }
            else if (clampAngle <= -180f)
            {
                clampAngle += 360f;
            }

            return clampAngle;
        }

        public static void QuickSort(Span<float> span)
        {
            QuickSort(span, 0, span.Length - 1);
        }

        private static void QuickSort(Span<float> span, int left, int right)
        {
            if (left < right)
            {
                int pivotIndex = Partition(span, left, right);
                QuickSort(span, left, pivotIndex - 1);
                QuickSort(span, pivotIndex + 1, right);
            }
        }

        private static int Partition(Span<float> span, int left, int right)
        {
            var pivot = span[right];
            int i = left - 1;

            for (int j = left; j < right; j++)
            {
                if (span[j] < pivot)
                {
                    i++;
                    Swap(span, i, j);
                }
            }

            Swap(span, i + 1, right);
            return i + 1;
        }

        private static void Swap(Span<float> span, int i, int j)
        {
            (span[i], span[j]) = (span[j], span[i]);
        }
        
        #region Polygon

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InsidePoint(this PolygonArea area, in Vector3 point)
        {
            if (point.y > (area.Anchor.y + area.UpperHeight) || point.y < (area.Anchor.y - area.LowerHeight))
                return false;

            ReadOnlySpan<Vector3> vertices = area.Vertices;
            return vertices.InsidePoint(point);
        }

        /// <summary>
        /// 获取平均点 
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetCenter(this ReadOnlySpan<Vector3> points)
        {
            if (null == points || 0 == points.Length)
            {
                return Vector3.zero;
            }

            int count = points.Length;
            if (1 < count)
            {
                Vector3 sum = Vector3.zero;
                foreach (Vector3 point in points)
                {
                    sum += point;
                }

                return sum / count;
            }

            return points[0];
        }

        /// <summary>
        ///  判断一个点是否在多边形内
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InsidePoint(this ReadOnlySpan<Vector3> polygon, in Vector3 point)
        {
            bool inner = false;
            int vertexCount = polygon.Length;

            for (var i = 0; i < vertexCount; i++)
            {
                ref readonly Vector3 v1 = ref polygon[i];
                ref readonly Vector3 v2 = ref polygon[(i + 1) % vertexCount];

                if ((v1.z <= point.z && v2.z > point.z) || (v1.z > point.z && v2.z <= point.z))
                {
                    if (point.x <= v1.x + (point.z - v1.z) / (v2.z - v1.z) * (v2.x - v1.x))
                    {
                        inner = !inner;
                    }
                }
            }

            return inner;
        }

        /// <summary>
        /// 判断一个点是否在多边形内
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InsidePoint(this ReadOnlySpan<Vector2> polygon, in Vector2 point)
        {
            bool inner = false;
            int vertexCount = polygon.Length;

            for (var i = 0; i < vertexCount; i++)
            {
                ref readonly Vector2 v1 = ref polygon[i];
                ref readonly Vector2 v2 = ref polygon[(i + 1) % vertexCount];

                if ((v1.y <= point.y && v2.y > point.y) || (v1.y > point.y && v2.y <= point.y))
                {
                    if (point.x <= v1.x + (point.y - v1.y) / (v2.y - v1.y) * (v2.x - v1.x))
                    {
                        inner = !inner;
                    }
                }
            }

            return inner;
        }
        
        /// <summary>
        /// 判断一个点是否在多边形内
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InsidePoint(this FastList<Vector2> polygon, in Vector2 point)
        {
            bool inner = false;
            int vertexCount = polygon.Count;

            for (var i = 0; i < vertexCount; i++)
            {
                ref readonly Vector2 v1 = ref polygon[i];
                ref readonly Vector2 v2 = ref polygon[(i + 1) % vertexCount];

                if ((v1.y <= point.y && v2.y > point.y) || (v1.y > point.y && v2.y <= point.y))
                {
                    if (point.x <= v1.x + (point.y - v1.y) / (v2.y - v1.y) * (v2.x - v1.x))
                    {
                        inner = !inner;
                    }
                }
            }

            return inner;
        }

        public static Vector2 ClosedPoint(this ReadOnlySpan<Vector2> polygon, in Vector2 point, in Vector2 defaultPos, in float radius)
        {
            Vector2 closedPoint = point;
            float closedSqrDis = float.MaxValue;
            bool validDis = false;
            
            int length = polygon.Length - 1;
            for (int i = 0; i < length; i++)
            {
                ref readonly Vector2 pi = ref polygon[i];
                ref readonly Vector2 pj = ref polygon[(i + 1) % length];

                Vector2 line_ij = (pj - pi).normalized;
                Vector2 line_ip = (point - pi).normalized;
                Vector2 line_jp = (point - pj).normalized;

                float dot1 = Vector2.Dot(line_ip, line_ij);
                float dot2 = Vector2.Dot(line_jp, -line_ij);
                if (dot1 > 0f && dot2 > 0f)
                {
                    Vector2 close = pi + Project(line_ip, line_ij);
                    float sqrDis = Vector2.SqrMagnitude(close - point);
                    if (sqrDis < closedSqrDis)
                    {
                        closedSqrDis = sqrDis;
                        closedPoint = close;
                        validDis = true;
                    }
                }
            }

            if (validDis)
            {
                closedPoint = Vector2.MoveTowards(closedPoint, defaultPos, radius);
            }
            else
            {
                // 最近点
                for (int i = 0; i <= length; i++)
                {
                    ref readonly Vector2 pi = ref polygon[i];
                    float sqrDis = Vector2.SqrMagnitude(pi - point);
                    if (sqrDis < closedSqrDis)
                    {
                        closedSqrDis = sqrDis;
                        closedPoint = pi;
                    }
                }
            }

            closedPoint = Vector2.MoveTowards(closedPoint, defaultPos, radius);
            return closedPoint;
        }

        #endregion
    }

    public struct PolygonArea
    {
        public Vector3 Anchor;
        public Vector3[] Vertices;
        public float UpperHeight;
        public float LowerHeight;
    }
}