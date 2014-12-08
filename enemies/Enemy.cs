﻿using Otter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD31 {
	class Enemy : Entity {

		private int health;
		private float cooldownTimer;
		private bool hurt = false;

		protected Vector2 acceleration = Vector2.Zero;
		protected Vector2 velocity = Vector2.Zero;
		protected float maxspeed = 5.0f;
		protected float minspeed = 0.0f;

		protected Vector2 direction = new Vector2(0, -1);
		private float angle;
		protected float dirStepAmount = 5.0f;

		protected Light light;

		public Enemy(float x, float y, int health, float cooldown) : base(x, y) {
			this.health = health;
			cooldownTimer = cooldown;
		}

		public override void Update() {
			velocity.X += acceleration.X;
			velocity.Y += acceleration.Y;

			var magnitude = Util.Clamp(velocity.Length, minspeed, maxspeed);
			if (magnitude != velocity.Length) {
				velocity.Normalize();
				velocity *= magnitude;
			}

			X += velocity.X;
			Y += velocity.Y;

			Wrap();
		}

		public void Kill() {
			ApplyDamage(1000);
		}

		public void ApplyDamage(int damage) {
			if (!hurt) {
				health -= damage;
				if (health > 0) {
					Game.Coroutine.Start(DamageCooldown());
				} else {
					Game.Coroutine.Start(Die());
				}
			}
		}

		IEnumerator DamageCooldown() {
			hurt = true;
			yield return Coroutine.Instance.WaitForSeconds(cooldownTimer);
			hurt = false;
		}

		virtual protected IEnumerator Death() {
			yield return 0;
		}

		IEnumerator Die() {
			SetHitbox(0, 0, -1);
			yield return Death();
			RemoveSelf();
		}

		void Wrap() {
			var left = 0 - ((int)Hitbox.Width >> 1);
			var right = 1920 + ((int)Hitbox.Width >> 1);

			var top = 0 - ((int)Hitbox.Height >> 1);
			var bottom = 1080 + ((int)Hitbox.Height >> 1);

			if (X < left) {
				X = right;
			} else if (X > right) {
				X = left;
			}

			if (Y < top) {
				Y = bottom;
			} else if (Y > bottom) {
				Y = top;
			}
		}

		protected float OrientCircularSprite() {
			Console.WriteLine(acceleration);

			if ((Math.Abs(acceleration.X) > 0.0f) || (Math.Abs(acceleration.Y) > 0.0f)) {
				var newAngle = Util.RAD_TO_DEG * (float)Math.Atan2(-acceleration.Y, acceleration.X);
				var angleDiff = ((((newAngle - angle) % 360) + 540) % 360) - 180;
				var rotateAmount = Util.Clamp(angleDiff, -dirStepAmount, dirStepAmount);
				direction = Util.Rotate(direction, rotateAmount);
				angle = (float)Math.Atan2(-direction.Y, direction.X) * Util.RAD_TO_DEG;
			}

			return angle;
		}

	}
}
