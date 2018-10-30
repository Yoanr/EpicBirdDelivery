#ifndef BLEND_MODES_CGINC
#define BLEND_MODES_CGINC

// a = couleur de l'objet
// b = couleur du fond

// formules trouvées ici : http://wwwimages.adobe.com/www.adobe.com/content/dam/Adobe/en/devnet/pdf/pdfs/PDF32000_2008.pdf page 324

fixed4 Normal(fixed4 a, fixed4 b)
{
	return a;
}

fixed4 Multiply(fixed4 a, fixed4 b)
{
	fixed4 r = a * b;
	r.a = a.a;
	return r;
}

fixed4 Screen(fixed4 a, fixed4 b)
{
	fixed4 r = 1.0 - (1.0 - b) * (1.0 - a);
	r.a = a.a;
	return r;
}

fixed4 Overlay(fixed4 a, fixed4 b)
{
	fixed4 r = b < .5 ? 2 * a*b : 1 - ((1 - a)*(2 - 2 * b));
	r.a = a.a;
	return r;
}

fixed4 Darken(fixed4 a, fixed4 b)
{
	fixed4 r = min(a, b);
	r.a = a.a;
	return r;
}

fixed4 Lighten(fixed4 a, fixed4 b)
{
	fixed4 r = max(a, b);
	r.a = a.a;
	return r;
}

fixed4 ColorDodge(fixed4 a, fixed4 b)
{
	fixed4 r = a < 1 ? min(1, b / (1 - a)) : 1;
	r.a = a.a;
	return r;
}

fixed4 ColorBurn(fixed4 a, fixed4 b)
{
	fixed4 r = a > 0 ? 1 - min(1, (1 - b) / a) : 0;
	r.a = a.a;
	return r;
}

fixed4 HardLight(fixed4 a, fixed4 b)
{
	fixed4 r = a <= .5 ? 2 * a*b : 1 - ((1 - b)*(2 - 2 * a));
	r.a = a.a;
	return r;
}

fixed4 SoftLight(fixed4 a, fixed4 b)
{
	fixed4 d = b <= .25 ? ((16 * b - 12)*b + 4)*b : sqrt(b);
	fixed4 r = a <= .5 ? b - (1 - 2 * a)*b*(1 - b) : b + (2 * a - 1)*(d - b);
	r.a = a.a;
	return r;
}

fixed4 Difference(fixed4 a, fixed4 b)
{
	fixed4 r = a > b ? a - b : b - a;
	r.a = a.a;
	return r;
}

fixed4 Exclusion(fixed4 a, fixed4 b)
{
	//fixed4 r = 0.5 - 2 * (a - 0.5)*(b - 0.5);
	fixed4 r = b + a - 2 * b*a;
	r.a = a.a;
	return r;
}

//fonctions pour la luminance et la saturation
//start

float Lum(fixed4 c)
{
	return .3 * c.r + .59 * c.g + .11 * c.b;
}

fixed4 ClipColor(fixed4 c)
{
	float l = Lum(c);
	float n = min(c.r, min(c.g, c.b));
	float x = max(c.r, max(c.g, c.b));
	fixed4 r = c;
	if (n < 0.0)
	{
		r.r = l + (((c.r - l) * l) / (l - n));
		r.g = l + (((c.g - l) * l) / (l - n));
		r.b = l + (((c.b - l) * l) / (l - n));
	}
	if (x > 1.0)
	{
		r.r = l + (((c.r - l)*(1 - l)) / (x - l));
		r.g = l + (((c.g - l)*(1 - l)) / (x - l));
		r.b = l + (((c.b - l)*(1 - l)) / (x - l));
	}
	return r;
}

fixed4 SetLum(fixed4 c, float l)
{
	float d = l - Lum(c);
	fixed4 r = c;
	r.r = c.r + d;
	r.g = c.g + d;
	r.b = c.b + d;
	return ClipColor(r);
}

float Sat(fixed4 c)
{
	return max(c.r, max(c.g, c.b)) - min(c.r, min(c.g, c.b));
}

fixed4 SetSat(fixed4 c, float s)
{
	fixed4 r = c;
	if (c.r > c.b && c.r > c.g)
	{
		if (c.g > c.b)
		{
			//c.r>c.g>c.b
			if (c.r > c.b)
			{
				r.g = (((c.g - c.b)*s) / (c.r - c.b));
				r.r = s;
			}
			else
			{
				r.g = 0;
				r.r = 0;
			}
			r.b = 0;
			return r;
		}
		else
		{
			//c.r>c.b>=c.g
			if (c.r > c.g)
			{
				r.b = (((c.b - c.g)*s) / (c.r - c.g));
				r.r = s;
			}
			else
			{
				r.b = 0;
				r.r = 0;
			}
			r.g = 0;
			return r;
		}
	}
	else if (c.g > c.r && c.g > c.b)
	{
		if (c.r > c.b)
		{
			//c.g>c.r>c.b
			if (c.g > c.b)
			{
				r.r = (((c.r - c.b)*s) / (c.g - c.b));
				r.g = s;
			}
			else
			{
				r.r = 0;
				r.g = 0;
			}
			r.b = 0;
			return r;
		}
		else
		{
			//c.g>c.b>=c.r
			if (c.g > c.r)
			{
				r.b = (((c.b - c.r)*s) / (c.g - c.r));
				r.g = s;
			}
			else
			{
				r.b = 0;
				r.g = 0;
			}
			r.r = 0;
			return r;
		}
	}
	else
	{
		if (c.r > c.g)
		{
			///c.b>=c.r>c.g
			if (c.b > c.g)
			{
				r.r = (((c.r - c.g)*s) / (c.b - c.g));
				r.b = s;
			}
			else
			{
				r.r = 0;
				r.b = 0;
			}
			r.g = 0;
			return r;
		}
		else
		{
			///c.b>=c.g>=c.r
			if (c.b > c.r)
			{
				r.g = (((c.g - c.r)*s) / (c.b - c.r));
				r.b = s;
			}
			else
			{
				r.g = 0;
				r.b = 0;
			}
			r.r = 0;
			return r;
		}
	}
}

//end

fixed4 Hue(fixed4 a, fixed4 b)
{
	fixed4 r = SetLum(SetSat(a, Sat(b)), Lum(b));
	r.a = a.a;
	return r;
}

fixed4 Saturation(fixed4 a, fixed4 b)
{
	fixed4 r = SetLum(SetSat(b, Sat(a)), Lum(b));
	r.a = a.a;
	return r;
}

fixed4 Color(fixed4 a, fixed4 b)
{
	fixed4 r = SetLum(a, Lum(b));
	r.a = a.a;
	return r;
}

fixed4 Luminosity(fixed4 a, fixed4 b)
{
	fixed4 r = SetLum(b, Lum(a));
	r.a = a.a;
	return r;
}

// autres formules trouvées ici  : http://zhur74.livejournal.com/15544.html (/!\ en russe)
// dans le site C = a et S = b

fixed4 VividLight(fixed4 a, fixed4 b)
{
	fixed4 r;
	r = (a <= .5) ?
		(((b + 2.0 * a) < 1.0) ? 0 : (b - 2.0 * (.5 - a)) / (2 * a)) :
		(((b + 2.0 * a) < 1.0) ? 1 : b / (1 - 2.0 * (a - .5)));
	r.a = a.a;
	return r;
}

fixed4  LinearLight(fixed4 a, fixed4 b)
{
	fixed4 r;
	r = (a <= .5) ?
		(((b + 2.0 * a) < 1.0) ? 0 : b - 2.0 * (.5 - a)) :
		(((b + 2.0 * a) < 1.0) ? 1 : b + 2.0 * (a - .5));
	r.a = a.a;
	return r;
}

fixed4  PinLight(fixed4 a, fixed4 b)
{
	fixed4 r;
	r = (a < .5) ?
		((b <= 2.0 * a) ? b : 2.0 * a) :
		((b >= 2.0 * (a - .5)) ? b : 2.0 * (a - .5));
	r.a = a.a;
	return r;
}

// autres formules trouvées ici  : http://zhur74.livejournal.com/17188.html (/!\ en russe)
// dans le site C = a et S = b

fixed4  HardMix(fixed4 a, fixed4 b)
{
	fixed4 r;
	r = (a < .5) ?
		((a + b < 1) ? 0 : 1) :
		((a + b <= 1) ? 0 : 1);
	r.a = a.a;
	return r;
}

//autres formules trouvées ici : http://photoblogstop.com/photoshop/photoshop-blend-modes-explained

fixed4 LinearBurn(fixed4 a, fixed4 b)
{
	fixed4 r = a + b - 1;
	r.a = a.a;
	return r;
}

fixed4 LinearDodge(fixed4 a, fixed4 b)
{
	fixed4 r = a + b;
	r.a = a.a;
	return r;
}

fixed4 Substract(fixed4 a, fixed4 b)
{
	fixed4 r = b - a;
	r.a = a.a;
	return r;
}

fixed4 Divide(fixed4 a, fixed4 b)
{
	fixed4 r = b / a;
	r.a = a.a;
	return r;
}

#endif