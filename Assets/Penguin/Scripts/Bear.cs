using UnityEngine;

namespace Penguin.Scripts
{
    public class Bear : MonoBehaviour
    {
        [Tooltip("The walking speed")]
        public float bearSpeed;
        
        private float randomizedSpeed;
        private float nextActionTime = -1f;
        private Vector3 targetPosition;
        
        /// <summary>
        /// Called every timestep
        /// </summary>
        private void FixedUpdate()
        {
            if (bearSpeed > 0f)
            {
                Walk();
            }
        }
        
        private void Walk()
        {
            // If it's time for the next action, pick a new speed and destination
            // Else, walk toward the destination
            if (Time.fixedTime >= nextActionTime)
            {
                // Randomize the speed
                randomizedSpeed = bearSpeed * Random.Range(.5f, 1.5f);

                // Pick a random target
                targetPosition = PenguinArea.ChooseRandomPosition(transform.parent.position, -45f, 45f, 4f, 9f);

                // Rotate toward the target
                var position = transform.position;
                transform.rotation = Quaternion.LookRotation(targetPosition - position, Vector3.up);

                // Calculate the time to get there
                var timeToGetThere = Vector3.Distance(position, targetPosition) / randomizedSpeed;
                nextActionTime = Time.fixedTime + timeToGetThere;
            }
            else
            {
                // Make sure that the bear does not swim past the target
                Vector3 moveVector = randomizedSpeed * Time.fixedDeltaTime * transform.forward;
                if (moveVector.magnitude <= Vector3.Distance(transform.position, targetPosition))
                {
                    transform.position += moveVector;
                }
                else
                {
                    transform.position = targetPosition;
                    nextActionTime = Time.fixedTime;
                }
            }
        }
    }
}