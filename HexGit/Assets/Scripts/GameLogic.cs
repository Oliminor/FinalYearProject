using UnityEngine;

namespace GameLogic
{
    public class GameLogic : MonoBehaviour
    {
        public static float GetSlopeAngle(CapsuleCollider _capsuleCol, float _raysize, LayerMask _whatIsGround, Transform _transform)
        {
            float slopeAngle = 90;
            Vector3 raycastPoint = _capsuleCol.bounds.center;
            float playerHeight = _capsuleCol.height * _transform.localScale.y;
            float raySize = playerHeight * 0.5f + _raysize;

            if (Physics.Raycast(raycastPoint, Vector3.down, out RaycastHit hitinfo, raySize, _whatIsGround))
            {
                slopeAngle = Vector3.Angle(_transform.up, hitinfo.normal);
            }

            return slopeAngle;
        }


        public static bool IsGrounded(CapsuleCollider _capsuleCol, LayerMask _whatIsGround, Transform _transform)
        {
            float sphereSize = (_capsuleCol.radius * _transform.localScale.y) * 0.9f;
            float capsuleRadius = _capsuleCol.radius * _transform.localScale.y;
            Vector3 spherePosition = _capsuleCol.bounds.min + new Vector3(capsuleRadius, 0, capsuleRadius) + new Vector3(0, capsuleRadius - 0.15f, 0);

            bool _isGrounded = Physics.CheckSphere(spherePosition, sphereSize, _whatIsGround);

            return _isGrounded;
        }

        public static Vector3 GetCameraForwardVector(Vector3 _inputVector, Camera _camera)
        {
            Vector3 movementVector = new Vector3(_inputVector.x, 0, _inputVector.y);

            // Direction based on the camera rotation
            Vector3 cameraForward = Quaternion.Euler(0, _camera.gameObject.transform.eulerAngles.y, 0) * movementVector;

            return cameraForward;
        }

        /// <summary>
        /// Get the slope direction for the movement
        /// </summary>
        public static Vector3 GetSlopeDirection(CapsuleCollider _capsuleCol, Vector3 _inputVector, Camera _camera, LayerMask _whatIsGround, Transform _transform)
        {
            Vector3 movementVector = new Vector3(_inputVector.x, 0, _inputVector.y);

            // Direction based on the camera rotation
            Vector3 cameraForward = Quaternion.Euler(0, _camera.gameObject.transform.eulerAngles.y, 0) * movementVector;

            Vector3 direction = cameraForward;
            Vector3 raycastPoint = _capsuleCol.bounds.center;
            float raySize = _capsuleCol.height * _transform.localScale.y * 0.5f + 0.3f;

            if (Physics.Raycast(raycastPoint, Vector3.down, out RaycastHit hitinfo, raySize, _whatIsGround))
            {
                Vector3 forward = cameraForward;
                Vector3 up = new Vector3(0, 1.0f, 0);
                Vector3 right = Vector3.Cross(up.normalized, forward.normalized);
                direction = Vector3.Cross(right, hitinfo.normal).normalized;
            }

            return direction;
        }

        /// <summary>
        /// Faces the player on Y angle to given position
        /// </summary>
        public static Quaternion FacePlayerTo(Vector3 _lookAt, Transform _transform)
        {
            Quaternion lookRotation = Quaternion.LookRotation((_lookAt - _transform.position).normalized);
            Vector3 angle = lookRotation.eulerAngles;
            angle = new Vector3(0, angle.y, 0);
            return Quaternion.Euler(angle);
        }

        /// <summary>
        ///  returns forward (Wall) Angle 
        /// </summary>
        public static float GetForwardAngle(CapsuleCollider _capsuleCol, LayerMask _whatIsGround, Transform _transform)
        {
            float forwardAngle = 0;
            float raySize = 0.5f;
            float capsuleRadius = _capsuleCol.radius * _transform.localScale.y;
            Vector3 raycastPoint = _capsuleCol.bounds.min + new Vector3(capsuleRadius, 0, capsuleRadius) + new Vector3(0, 0.3f, 0);

            if (Physics.Raycast(raycastPoint, Vector3.forward, out RaycastHit hitinfo, raySize, _whatIsGround))
            {
                forwardAngle = Vector3.Angle(_transform.up, hitinfo.normal);
            }

            return forwardAngle;
        }

        public static bool IsInAir(float _raySizeAddition, CapsuleCollider _capsuleCol, Transform _transform)
        {
            float playerHeight = _capsuleCol.height * _transform.localScale.y;
            Vector3 raycastPoint = _capsuleCol.bounds.center;
            float raySize = playerHeight * 0.5f + _raySizeAddition;

            Debug.DrawRay(raycastPoint, Vector3.down * raySize, Color.magenta);

            bool IsInAir = Physics.Raycast(raycastPoint, Vector3.down, raySize);
            IsInAir = !IsInAir;

            return IsInAir;
        }
    }
}
