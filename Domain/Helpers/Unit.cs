namespace Domain.Helpers;

public readonly struct Unit : IEquatable<Unit>, IComparable<Unit>
{
    public static readonly Unit Default = default;

    public override int GetHashCode() => 0;

    public override bool Equals(object? obj) => obj is Unit;

    public bool Equals(Unit other) => true;

    public override string ToString() => "()";

    public static bool operator ==(Unit lhs, Unit rhs) => true;

    public static bool operator !=(Unit lhs, Unit rhs) => false;

    public static bool operator >(Unit lhs, Unit rhs) => false;

    public static bool operator >=(Unit lhs, Unit rhs) => true;

    public static bool operator <(Unit lhs, Unit rhs) => false;

    public static bool operator <=(Unit lhs, Unit rhs) => true;

    public int CompareTo(Unit other) => 0;

    public static Unit operator +(Unit a, Unit b) => Default;
}
