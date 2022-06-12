using System;

public struct InputData : IEquatable<InputData>
{
    public int direction;
    public bool aPressed;
    public bool bPressed;
    public bool cPressed;
    public bool jumpPressed;

    public string Code
    {
        get
        {
            string output = $"{direction}";
            if (aPressed)
                output += "a";
            if (bPressed)
                output += "b";
            if (cPressed)
                output += "c";
            return output;
        }
    }

    public override bool Equals(object obj) => obj is InputData other && Equals(other);

    public bool Equals(InputData other)
    {
        if (Object.ReferenceEquals(this, other))
        {
            return true;
        }

        if (GetType() != other.GetType())
        {
            return false;
        }

        return other is InputData data &&
                direction == data.direction &&
                aPressed == data.aPressed &&
                bPressed == data.bPressed &&
                cPressed == data.cPressed &&
                jumpPressed == data.jumpPressed;
    }

    public static bool operator ==(InputData lhs, InputData rhs)
    {
        return lhs.Equals(rhs);
    }

    public static bool operator !=(InputData lhs, InputData rhs)
    {
        return !lhs.Equals(rhs);
    }

    public override int GetHashCode()
    {
        int hashCode = -2132171370;
        hashCode = hashCode * -1521134295 + direction.GetHashCode();
        hashCode = hashCode * -1521134295 + aPressed.GetHashCode();
        hashCode = hashCode * -1521134295 + bPressed.GetHashCode();
        hashCode = hashCode * -1521134295 + cPressed.GetHashCode();
        hashCode = hashCode * -1521134295 + jumpPressed.GetHashCode();
        return hashCode;
    }
}
