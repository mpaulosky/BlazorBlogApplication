using System;

namespace Shared.TestStubs
{
    // Minimal ObjectId substitute for tests migrating away from MongoDB
    public readonly struct ObjectId
    {
        private readonly Guid _value;

        private ObjectId(Guid g) { _value = g; }

        public static ObjectId GenerateNewId() => new ObjectId(Guid.NewGuid());

        public Guid ToGuid() => _value;

        public static implicit operator Guid(ObjectId id) => id._value;

        public override string ToString() => _value.ToString();
    }
}
