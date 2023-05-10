using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Status {
    public static class JumpStatus {
        public static bool IsFallingRB(Rigidbody rb) {
            return rb.velocity.y < -0.1f;
        }

        public static bool IsFallingCC(CharacterController c) {
            return c.velocity.y < -0.1f;
        }

        public static bool IsJumpingRB(Rigidbody rb) {
            return rb.velocity.y > 0.1f;
        }

        public static bool IsGrounded(Transform t) {
            bool canJump = Physics.Raycast(t.position, Vector3.down, out RaycastHit hit, 1f) && hit.collider.CompareTag("Ground");
            return canJump;
        }
    }
}
