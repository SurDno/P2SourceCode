using UnityEngine;

public static class EasingFunction {
	private const float NATURAL_LOG_OF_2 = 0.6931472f;

	public static float Linear(float start, float end, float value) {
		return Mathf.Lerp(start, end, value);
	}

	public static float Spring(float start, float end, float value) {
		value = Mathf.Clamp01(value);
		value =
			(float)((Mathf.Sin(
					(float)(value * 3.1415927410125732 * (0.20000000298023224 + 2.5 * value * value * value))) *
				(double)Mathf.Pow(1f - value, 2.2f) + value) * (1.0 + 1.2000000476837158 * (1.0 - value)));
		return start + (end - start) * value;
	}

	public static float EaseInQuad(float start, float end, float value) {
		end -= start;
		return end * value * value + start;
	}

	public static float EaseOutQuad(float start, float end, float value) {
		end -= start;
		return (float)(-(double)end * value * (value - 2.0)) + start;
	}

	public static float EaseInOutQuad(float start, float end, float value) {
		value /= 0.5f;
		end -= start;
		if (value < 1.0)
			return end * 0.5f * value * value + start;
		--value;
		return (float)(-(double)end * 0.5 * (value * (value - 2.0) - 1.0)) + start;
	}

	public static float EaseInCubic(float start, float end, float value) {
		end -= start;
		return end * value * value * value + start;
	}

	public static float EaseOutCubic(float start, float end, float value) {
		--value;
		end -= start;
		return end * (float)(value * (double)value * value + 1.0) + start;
	}

	public static float EaseInOutCubic(float start, float end, float value) {
		value /= 0.5f;
		end -= start;
		if (value < 1.0)
			return end * 0.5f * value * value * value + start;
		value -= 2f;
		return (float)(end * 0.5 * (value * (double)value * value + 2.0)) + start;
	}

	public static float EaseInQuart(float start, float end, float value) {
		end -= start;
		return end * value * value * value * value + start;
	}

	public static float EaseOutQuart(float start, float end, float value) {
		--value;
		end -= start;
		return (float)(-(double)end * (value * (double)value * value * value - 1.0)) + start;
	}

	public static float EaseInOutQuart(float start, float end, float value) {
		value /= 0.5f;
		end -= start;
		if (value < 1.0)
			return end * 0.5f * value * value * value * value + start;
		value -= 2f;
		return (float)(-(double)end * 0.5 * (value * (double)value * value * value - 2.0)) + start;
	}

	public static float EaseInQuint(float start, float end, float value) {
		end -= start;
		return end * value * value * value * value * value + start;
	}

	public static float EaseOutQuint(float start, float end, float value) {
		--value;
		end -= start;
		return end * (float)(value * (double)value * value * value * value + 1.0) + start;
	}

	public static float EaseInOutQuint(float start, float end, float value) {
		value /= 0.5f;
		end -= start;
		if (value < 1.0)
			return end * 0.5f * value * value * value * value * value + start;
		value -= 2f;
		return (float)(end * 0.5 * (value * (double)value * value * value * value + 2.0)) + start;
	}

	public static float EaseInSine(float start, float end, float value) {
		end -= start;
		return -end * Mathf.Cos(value * 1.57079637f) + end + start;
	}

	public static float EaseOutSine(float start, float end, float value) {
		end -= start;
		return end * Mathf.Sin(value * 1.57079637f) + start;
	}

	public static float EaseInOutSine(float start, float end, float value) {
		end -= start;
		return (float)(-(double)end * 0.5 * (Mathf.Cos(3.14159274f * value) - 1.0)) + start;
	}

	public static float EaseInExpo(float start, float end, float value) {
		end -= start;
		return end * Mathf.Pow(2f, (float)(10.0 * (value - 1.0))) + start;
	}

	public static float EaseOutExpo(float start, float end, float value) {
		end -= start;
		return end * (float)(-(double)Mathf.Pow(2f, -10f * value) + 1.0) + start;
	}

	public static float EaseInOutExpo(float start, float end, float value) {
		value /= 0.5f;
		end -= start;
		if (value < 1.0)
			return end * 0.5f * Mathf.Pow(2f, (float)(10.0 * (value - 1.0))) + start;
		--value;
		return (float)(end * 0.5 * (-(double)Mathf.Pow(2f, -10f * value) + 2.0)) + start;
	}

	public static float EaseInCirc(float start, float end, float value) {
		end -= start;
		return (float)(-(double)end * (Mathf.Sqrt((float)(1.0 - value * (double)value)) - 1.0)) + start;
	}

	public static float EaseOutCirc(float start, float end, float value) {
		--value;
		end -= start;
		return end * Mathf.Sqrt((float)(1.0 - value * (double)value)) + start;
	}

	public static float EaseInOutCirc(float start, float end, float value) {
		value /= 0.5f;
		end -= start;
		if (value < 1.0)
			return (float)(-(double)end * 0.5 * (Mathf.Sqrt((float)(1.0 - value * (double)value)) - 1.0)) + start;
		value -= 2f;
		return (float)(end * 0.5 * (Mathf.Sqrt((float)(1.0 - value * (double)value)) + 1.0)) + start;
	}

	public static float EaseInBounce(float start, float end, float value) {
		end -= start;
		var num = 1f;
		return end - EaseOutBounce(0.0f, end, num - value) + start;
	}

	public static float EaseOutBounce(float start, float end, float value) {
		value /= 1f;
		end -= start;
		if (value < 0.36363637447357178)
			return end * (121f / 16f * value * value) + start;
		if (value < 0.72727274894714355) {
			value -= 0.545454562f;
			return end * (float)(121.0 / 16.0 * value * value + 0.75) + start;
		}

		if (value < 10.0 / 11.0) {
			value -= 0.8181818f;
			return end * (float)(121.0 / 16.0 * value * value + 15.0 / 16.0) + start;
		}

		value -= 0.954545438f;
		return end * (float)(121.0 / 16.0 * value * value + 63.0 / 64.0) + start;
	}

	public static float EaseInOutBounce(float start, float end, float value) {
		end -= start;
		var num = 1f;
		return value < num * 0.5
			? EaseInBounce(0.0f, end, value * 2f) * 0.5f + start
			: (float)(EaseOutBounce(0.0f, end, value * 2f - num) * 0.5 + end * 0.5) + start;
	}

	public static float EaseInBack(float start, float end, float value) {
		end -= start;
		value /= 1f;
		var num = 1.70158f;
		return (float)(end * (double)value * value * ((num + 1.0) * value - num)) + start;
	}

	public static float EaseOutBack(float start, float end, float value) {
		var num = 1.70158f;
		end -= start;
		--value;
		return end * (float)(value * (double)value * ((num + 1.0) * value + num) + 1.0) + start;
	}

	public static float EaseInOutBack(float start, float end, float value) {
		var num1 = 1.70158f;
		end -= start;
		value /= 0.5f;
		if (value < 1.0) {
			var num2 = num1 * 1.525f;
			return (float)(end * 0.5 * (value * (double)value * ((num2 + 1.0) * value - num2))) + start;
		}

		value -= 2f;
		var num3 = num1 * 1.525f;
		return (float)(end * 0.5 * (value * (double)value * ((num3 + 1.0) * value + num3) + 2.0)) + start;
	}

	public static float EaseInElastic(float start, float end, float value) {
		end -= start;
		var num1 = 1f;
		var num2 = num1 * 0.3f;
		var num3 = 0.0f;
		if (value == 0.0)
			return start;
		if ((value /= num1) == 1.0)
			return start + end;
		float num4;
		if (num3 == 0.0 || num3 < (double)Mathf.Abs(end)) {
			num3 = end;
			num4 = num2 / 4f;
		} else
			num4 = num2 / 6.28318548f * Mathf.Asin(end / num3);

		return (float)-(num3 * (double)Mathf.Pow(2f, 10f * --value) *
		                Mathf.Sin((float)((value * (double)num1 - num4) * 6.2831854820251465) / num2)) + start;
	}

	public static float EaseOutElastic(float start, float end, float value) {
		end -= start;
		var num1 = 1f;
		var num2 = num1 * 0.3f;
		var num3 = 0.0f;
		if (value == 0.0)
			return start;
		if ((value /= num1) == 1.0)
			return start + end;
		float num4;
		if (num3 == 0.0 || num3 < (double)Mathf.Abs(end)) {
			num3 = end;
			num4 = num2 * 0.25f;
		} else
			num4 = num2 / 6.28318548f * Mathf.Asin(end / num3);

		return num3 * Mathf.Pow(2f, -10f * value) *
			Mathf.Sin((float)((value * (double)num1 - num4) * 6.2831854820251465) / num2) + end + start;
	}

	public static float EaseInOutElastic(float start, float end, float value) {
		end -= start;
		var num1 = 1f;
		var num2 = num1 * 0.3f;
		var num3 = 0.0f;
		if (value == 0.0)
			return start;
		if ((value /= num1 * 0.5f) == 2.0)
			return start + end;
		float num4;
		if (num3 == 0.0 || num3 < (double)Mathf.Abs(end)) {
			num3 = end;
			num4 = num2 / 4f;
		} else
			num4 = num2 / 6.28318548f * Mathf.Asin(end / num3);

		return value < 1.0
			? (float)(-0.5 * (num3 * (double)Mathf.Pow(2f, 10f * --value) *
			                  Mathf.Sin((float)((value * (double)num1 - num4) * 6.2831854820251465) / num2))) + start
			: (float)(num3 * (double)Mathf.Pow(2f, -10f * --value) *
			          Mathf.Sin((float)((value * (double)num1 - num4) * 6.2831854820251465) / num2) * 0.5) + end +
			  start;
	}

	public static float LinearD(float start, float end, float value) {
		return end - start;
	}

	public static float EaseInQuadD(float start, float end, float value) {
		return (float)(2.0 * (end - (double)start)) * value;
	}

	public static float EaseOutQuadD(float start, float end, float value) {
		end -= start;
		return (float)(-(double)end * value - end * (value - 2.0));
	}

	public static float EaseInOutQuadD(float start, float end, float value) {
		value /= 0.5f;
		end -= start;
		if (value < 1.0)
			return end * value;
		--value;
		return end * (1f - value);
	}

	public static float EaseInCubicD(float start, float end, float value) {
		return (float)(3.0 * (end - (double)start)) * value * value;
	}

	public static float EaseOutCubicD(float start, float end, float value) {
		--value;
		end -= start;
		return 3f * end * value * value;
	}

	public static float EaseInOutCubicD(float start, float end, float value) {
		value /= 0.5f;
		end -= start;
		if (value < 1.0)
			return 1.5f * end * value * value;
		value -= 2f;
		return 1.5f * end * value * value;
	}

	public static float EaseInQuartD(float start, float end, float value) {
		return (float)(4.0 * (end - (double)start)) * value * value * value;
	}

	public static float EaseOutQuartD(float start, float end, float value) {
		--value;
		end -= start;
		return -4f * end * value * value * value;
	}

	public static float EaseInOutQuartD(float start, float end, float value) {
		value /= 0.5f;
		end -= start;
		if (value < 1.0)
			return 2f * end * value * value * value;
		value -= 2f;
		return -2f * end * value * value * value;
	}

	public static float EaseInQuintD(float start, float end, float value) {
		return (float)(5.0 * (end - (double)start)) * value * value * value * value;
	}

	public static float EaseOutQuintD(float start, float end, float value) {
		--value;
		end -= start;
		return 5f * end * value * value * value * value;
	}

	public static float EaseInOutQuintD(float start, float end, float value) {
		value /= 0.5f;
		end -= start;
		if (value < 1.0)
			return 2.5f * end * value * value * value * value;
		value -= 2f;
		return 2.5f * end * value * value * value * value;
	}

	public static float EaseInSineD(float start, float end, float value) {
		return (float)((end - (double)start) * 0.5 * 3.1415927410125732) * Mathf.Sin(1.57079637f * value);
	}

	public static float EaseOutSineD(float start, float end, float value) {
		end -= start;
		return 1.57079637f * end * Mathf.Cos(value * 1.57079637f);
	}

	public static float EaseInOutSineD(float start, float end, float value) {
		end -= start;
		return (float)(end * 0.5 * 3.1415927410125732) * Mathf.Sin(3.14159274f * value);
	}

	public static float EaseInExpoD(float start, float end, float value) {
		return (float)(6.9314718246459961 * (end - (double)start)) * Mathf.Pow(2f, (float)(10.0 * (value - 1.0)));
	}

	public static float EaseOutExpoD(float start, float end, float value) {
		end -= start;
		return 3.465736f * end * Mathf.Pow(2f, (float)(1.0 - 10.0 * value));
	}

	public static float EaseInOutExpoD(float start, float end, float value) {
		value /= 0.5f;
		end -= start;
		if (value < 1.0)
			return 3.465736f * end * Mathf.Pow(2f, (float)(10.0 * (value - 1.0)));
		--value;
		return 3.465736f * end / Mathf.Pow(2f, 10f * value);
	}

	public static float EaseInCircD(float start, float end, float value) {
		return (end - start) * value / Mathf.Sqrt((float)(1.0 - value * (double)value));
	}

	public static float EaseOutCircD(float start, float end, float value) {
		--value;
		end -= start;
		return -end * value / Mathf.Sqrt((float)(1.0 - value * (double)value));
	}

	public static float EaseInOutCircD(float start, float end, float value) {
		value /= 0.5f;
		end -= start;
		if (value < 1.0)
			return (float)(end * (double)value / (2.0 * Mathf.Sqrt((float)(1.0 - value * (double)value))));
		value -= 2f;
		return (float)(-(double)end * value / (2.0 * Mathf.Sqrt((float)(1.0 - value * (double)value))));
	}

	public static float EaseInBounceD(float start, float end, float value) {
		end -= start;
		var num = 1f;
		return EaseOutBounceD(0.0f, end, num - value);
	}

	public static float EaseOutBounceD(float start, float end, float value) {
		value /= 1f;
		end -= start;
		if (value < 0.36363637447357178)
			return (float)(2.0 * end * (121.0 / 16.0)) * value;
		if (value < 0.72727274894714355) {
			value -= 0.545454562f;
			return (float)(2.0 * end * (121.0 / 16.0)) * value;
		}

		if (value < 10.0 / 11.0) {
			value -= 0.8181818f;
			return (float)(2.0 * end * (121.0 / 16.0)) * value;
		}

		value -= 0.954545438f;
		return (float)(2.0 * end * (121.0 / 16.0)) * value;
	}

	public static float EaseInOutBounceD(float start, float end, float value) {
		end -= start;
		var num = 1f;
		return value < num * 0.5
			? EaseInBounceD(0.0f, end, value * 2f) * 0.5f
			: EaseOutBounceD(0.0f, end, value * 2f - num) * 0.5f;
	}

	public static float EaseInBackD(float start, float end, float value) {
		var num = 1.70158f;
		return (float)(3.0 * (num + 1.0) * (end - (double)start) * value * value -
		               2.0 * num * (end - (double)start) * value);
	}

	public static float EaseOutBackD(float start, float end, float value) {
		var num = 1.70158f;
		end -= start;
		--value;
		return end * (float)((num + 1.0) * value * value + 2.0 * value * ((num + 1.0) * value + num));
	}

	public static float EaseInOutBackD(float start, float end, float value) {
		var num1 = 1.70158f;
		end -= start;
		value /= 0.5f;
		if (value < 1.0) {
			var num2 = num1 * 1.525f;
			return (float)(0.5 * end * (num2 + 1.0) * value * value +
			               end * (double)value * ((num2 + 1.0) * value - num2));
		}

		value -= 2f;
		var num3 = num1 * 1.525f;
		return (float)(0.5 * end * ((num3 + 1.0) * value * value + 2.0 * value * ((num3 + 1.0) * value + num3)));
	}

	public static float EaseInElasticD(float start, float end, float value) {
		return EaseOutElasticD(start, end, 1f - value);
	}

	public static float EaseOutElasticD(float start, float end, float value) {
		end -= start;
		var num1 = 1f;
		var num2 = num1 * 0.3f;
		var num3 = 0.0f;
		float num4;
		if (num3 == 0.0 || num3 < (double)Mathf.Abs(end)) {
			num3 = end;
			num4 = num2 * 0.25f;
		} else
			num4 = num2 / 6.28318548f * Mathf.Asin(end / num3);

		return (float)(num3 * 3.1415927410125732 * num1 * Mathf.Pow(2f, (float)(1.0 - 10.0 * value)) *
			Mathf.Cos((float)(6.2831854820251465 * (num1 * (double)value - num4)) / num2) / num2 - 3.465735912322998 *
			num3 * Mathf.Pow(2f, (float)(1.0 - 10.0 * value)) *
			Mathf.Sin((float)(6.2831854820251465 * (num1 * (double)value - num4)) / num2));
	}

	public static float EaseInOutElasticD(float start, float end, float value) {
		end -= start;
		var num1 = 1f;
		var num2 = num1 * 0.3f;
		var num3 = 0.0f;
		float num4;
		if (num3 == 0.0 || num3 < (double)Mathf.Abs(end)) {
			num3 = end;
			num4 = num2 / 4f;
		} else
			num4 = num2 / 6.28318548f * Mathf.Asin(end / num3);

		if (value < 1.0) {
			--value;
			return (float)(-3.465735912322998 * num3 * Mathf.Pow(2f, 10f * value) *
				Mathf.Sin((float)(6.2831854820251465 * (num1 * (double)value - 2.0)) / num2) - num3 *
				3.1415927410125732 * num1 * Mathf.Pow(2f, 10f * value) *
				Mathf.Cos((float)(6.2831854820251465 * (num1 * (double)value - num4)) / num2) / num2);
		}

		--value;
		return (float)(num3 * 3.1415927410125732 * num1 *
			Mathf.Cos((float)(6.2831854820251465 * (num1 * (double)value - num4)) / num2) /
			(num2 * (double)Mathf.Pow(2f, 10f * value)) - 3.465735912322998 * num3 *
			Mathf.Sin((float)(6.2831854820251465 * (num1 * (double)value - num4)) / num2) / Mathf.Pow(2f, 10f * value));
	}

	public static float SpringD(float start, float end, float value) {
		value = Mathf.Clamp01(value);
		end -= start;
		return (float)(end * (6.0 * (1.0 - value) / 5.0 + 1.0) *
			(-2.2000000476837158 * Mathf.Pow(1f - value, 1.2f) * Mathf.Sin((float)(3.1415927410125732 * value *
				 (2.5 * value * value * value + 0.20000000298023224))) +
			 Mathf.Pow(1f - value, 2.2f) *
			 (3.1415927410125732 * (2.5 * value * value * value + 0.20000000298023224) +
			  23.561944961547852 * value * value * value) *
			 Mathf.Cos((float)(3.1415927410125732 * value * (2.5 * value * value * value + 0.20000000298023224))) +
			 1.0) - 6.0 * end *
			(Mathf.Pow(1f - value, 2.2f) *
				(double)Mathf.Sin((float)(3.1415927410125732 * value *
				                          (2.5 * value * value * value + 0.20000000298023224))) + value / 5.0));
	}

	public static Function GetEasingFunction(Ease easingFunction) {
		switch (easingFunction) {
			case Ease.EaseInQuad:
				return EaseInQuad;
			case Ease.EaseOutQuad:
				return EaseOutQuad;
			case Ease.EaseInOutQuad:
				return EaseInOutQuad;
			case Ease.EaseInCubic:
				return EaseInCubic;
			case Ease.EaseOutCubic:
				return EaseOutCubic;
			case Ease.EaseInOutCubic:
				return EaseInOutCubic;
			case Ease.EaseInQuart:
				return EaseInQuart;
			case Ease.EaseOutQuart:
				return EaseOutQuart;
			case Ease.EaseInOutQuart:
				return EaseInOutQuart;
			case Ease.EaseInQuint:
				return EaseInQuint;
			case Ease.EaseOutQuint:
				return EaseOutQuint;
			case Ease.EaseInOutQuint:
				return EaseInOutQuint;
			case Ease.EaseInSine:
				return EaseInSine;
			case Ease.EaseOutSine:
				return EaseOutSine;
			case Ease.EaseInOutSine:
				return EaseInOutSine;
			case Ease.EaseInExpo:
				return EaseInExpo;
			case Ease.EaseOutExpo:
				return EaseOutExpo;
			case Ease.EaseInOutExpo:
				return EaseInOutExpo;
			case Ease.EaseInCirc:
				return EaseInCirc;
			case Ease.EaseOutCirc:
				return EaseOutCirc;
			case Ease.EaseInOutCirc:
				return EaseInOutCirc;
			case Ease.Linear:
				return Linear;
			case Ease.Spring:
				return Spring;
			case Ease.EaseInBounce:
				return EaseInBounce;
			case Ease.EaseOutBounce:
				return EaseOutBounce;
			case Ease.EaseInOutBounce:
				return EaseInOutBounce;
			case Ease.EaseInBack:
				return EaseInBack;
			case Ease.EaseOutBack:
				return EaseOutBack;
			case Ease.EaseInOutBack:
				return EaseInOutBack;
			case Ease.EaseInElastic:
				return EaseInElastic;
			case Ease.EaseOutElastic:
				return EaseOutElastic;
			case Ease.EaseInOutElastic:
				return EaseInOutElastic;
			default:
				return null;
		}
	}

	public static Function GetEasingFunctionDerivative(
		Ease easingFunction) {
		switch (easingFunction) {
			case Ease.EaseInQuad:
				return EaseInQuadD;
			case Ease.EaseOutQuad:
				return EaseOutQuadD;
			case Ease.EaseInOutQuad:
				return EaseInOutQuadD;
			case Ease.EaseInCubic:
				return EaseInCubicD;
			case Ease.EaseOutCubic:
				return EaseOutCubicD;
			case Ease.EaseInOutCubic:
				return EaseInOutCubicD;
			case Ease.EaseInQuart:
				return EaseInQuartD;
			case Ease.EaseOutQuart:
				return EaseOutQuartD;
			case Ease.EaseInOutQuart:
				return EaseInOutQuartD;
			case Ease.EaseInQuint:
				return EaseInQuintD;
			case Ease.EaseOutQuint:
				return EaseOutQuintD;
			case Ease.EaseInOutQuint:
				return EaseInOutQuintD;
			case Ease.EaseInSine:
				return EaseInSineD;
			case Ease.EaseOutSine:
				return EaseOutSineD;
			case Ease.EaseInOutSine:
				return EaseInOutSineD;
			case Ease.EaseInExpo:
				return EaseInExpoD;
			case Ease.EaseOutExpo:
				return EaseOutExpoD;
			case Ease.EaseInOutExpo:
				return EaseInOutExpoD;
			case Ease.EaseInCirc:
				return EaseInCircD;
			case Ease.EaseOutCirc:
				return EaseOutCircD;
			case Ease.EaseInOutCirc:
				return EaseInOutCircD;
			case Ease.Linear:
				return LinearD;
			case Ease.Spring:
				return SpringD;
			case Ease.EaseInBounce:
				return EaseInBounceD;
			case Ease.EaseOutBounce:
				return EaseOutBounceD;
			case Ease.EaseInOutBounce:
				return EaseInOutBounceD;
			case Ease.EaseInBack:
				return EaseInBackD;
			case Ease.EaseOutBack:
				return EaseOutBackD;
			case Ease.EaseInOutBack:
				return EaseInOutBackD;
			case Ease.EaseInElastic:
				return EaseInElasticD;
			case Ease.EaseOutElastic:
				return EaseOutElasticD;
			case Ease.EaseInOutElastic:
				return EaseInOutElasticD;
			default:
				return null;
		}
	}

	public enum Ease {
		EaseInQuad,
		EaseOutQuad,
		EaseInOutQuad,
		EaseInCubic,
		EaseOutCubic,
		EaseInOutCubic,
		EaseInQuart,
		EaseOutQuart,
		EaseInOutQuart,
		EaseInQuint,
		EaseOutQuint,
		EaseInOutQuint,
		EaseInSine,
		EaseOutSine,
		EaseInOutSine,
		EaseInExpo,
		EaseOutExpo,
		EaseInOutExpo,
		EaseInCirc,
		EaseOutCirc,
		EaseInOutCirc,
		Linear,
		Spring,
		EaseInBounce,
		EaseOutBounce,
		EaseInOutBounce,
		EaseInBack,
		EaseOutBack,
		EaseInOutBack,
		EaseInElastic,
		EaseOutElastic,
		EaseInOutElastic
	}

	public delegate float Function(float s, float e, float v);
}