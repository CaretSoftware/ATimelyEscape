using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
	
// Use namespaces when it makes sense
namespace Standard {
	/* Object with this attribute gets selected when clicked in scene.
	 * Convenient when wanting to select the base transform in hierarchy -
	 * (OR if you don't want parent transform to be selected)
	 * Place this Attribute [SelectionBase] on script in 
	 */
	[SelectionBase]
	// Use where a component is required for script to work, gets added to GameObject when script dragged onto it 
	[RequireComponent(typeof(Camera))]
	public class CodeStandard : MonoBehaviour { // Inline curly brackets (change your IDE settings)
		/*	A note on comments:
		 * Only English characters in comments, write in english! (Several reasons, one is UTF character encoding, other is readability)
		 * Before commenting; ask yourself if the method/variable name can be changed to explain what it does instead. 
		 * Google: "Self Documenting Code"
		 * Your code should be clear enough without comments.
		 * Of course more advanced/intricate code can/should be commented.
		 * Comments in code while writing, debugging and testing is of course to be expected.
		 * Try holding comments within the right margin line (120 characters). A little overspill is ok.
		 * This document is horrible for how many comments there are! I cannot see the code for all the comments!
		 */

		private const string StringlyTypedValue = "string";	// make variables for "stringly" typed variables e.g. Input.GetAxisRaw("Horizontal");
		
		// group class variables together, separate with whitespaces when it makes sense. Comment sections if needed
		private const float ConstantNumber = .1f; // type out decimals for clarity, integer part can be left out if 0

		private Transform _transform; // Cache transform for optimization, underscore private variables

		public float Number { get; private set; } // Properties for simple getters

		[SerializeField, Tooltip("Turn true to make object move upwards!")] // Attributes on line above class variable
		private bool active = false; // don't underscore [SerializeField] private variables, treat like public

		[SerializeField] private List<Vector3> positions;
			
		
		// Use start for initialising when possible. Use Awake when it needs to be initialised before something else 
		private void Start() {
			Number = ConstantNumber;
			// _transform ??= transform;		// Do _NOT_ use null-coalescing operators for Unity Components! They've overloaded the equals operator for UnityEngine.Object and broke this
			_transform = transform;				// cache transforms for performance
			positions ??= new List<Vector3>();	// This is fine!
		}

		private void Update() {
			if (!active) return; // invert logic to avoid nesting of if clauses

			// Keep Update loop "readable" with method calls
			// (WHAT is done every frame. If I wanna know HOW, I'll go and read the code there) 
			MakeTransformGoBRRRRRRR();
			ConstrainTransform();
		}

		private void MakeTransformGoBRRRRRRR() { // Use clear and easily typed naming for methods, (not like this!)

			if (IsAboveLimit()) return;

			/* Don't be afraid to declare primitive values and structs within methods.
			 * If they're primitive values or structs they are put on the stack
			 * and cleaned up directly after coming out of scope (Vector3 is a struct of three primitives).
			 * Classes and reference types are put on the heap and garbage collected (slow + frame hick-ups) cache!
			 * Structs are put on the stack, but values they contain can be reference types and are put on the heap —
			 * beware the difference
			 * Declaring within method increases readability of class and sometimes even help in performance
			 */

			// Place Vector3's last in calculations for performance (3 multiplications instead of 6)
			Vector3 upwardMovement = Number * Time.deltaTime * Vector3.up;
			_transform.position += upwardMovement;

			bool IsAboveLimit() { // Use Local Functions if only used within Method to improve readability
				return _transform.position.y > 100.0f;
			}
		}

		// If out of bounds, bring back to origin
		private void ConstrainTransform() {

			if (OutsideBounds()) { // Try to avoid not-statements e.g; !WithinBounds()
				_transform.position = Vector3.zero;
			}

			bool OutsideBounds() {
				// bounds is not a design value that we want changed,
				// and only pertains to this method,
				// don't clutter up class variables if not needed!
				const float bounds = 10_000.0f; // Use underscore as thousands separator to improve readability
				Vector3
					pos = _transform.position; // repeated property access of built in component is inefficient, cache

				// Stack long if conditions
				return
					pos.x > bounds &&
					pos.x < -bounds &&
					pos.z > bounds &&
					pos.z < -bounds;
			}
		}

		/// <summary>
		/// Write a CONCISE description for what your public method is meant to do if the Method name
		/// is not enough to describe it and it's meant to be used by other programmers frequently.
		/// (This description is TOO LONG!)
		/// (bool PositiveNumber is clear enough, so this is an example of when not to write a docs comment!)
		/// Don't write _how_ it works, write what it does!
		/// Doc comments brings up a description in the IDE and can be hovered over to explain the methods function.
		/// Try hovering over this one!
		/// Don't name Methods "PositiveNumber(int number)", parameter name makes "Number" in method name superfluous
		/// </summary>
		/// <param name="number">instruction for what value is expected here,
		/// use clear naming to help user figure out what parameter to use, preferably to read it as a sentence
		/// (e.g. Positive(number) ) return value bool makes "IsPositive" superfluous but you may use it.</param>
		/// <returns></returns>
		public bool Positive(int number) {
			return number >= 0;
		}
	}
}