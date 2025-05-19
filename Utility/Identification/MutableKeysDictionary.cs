using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace MC_BSR_S2_Calculator.Utility.Identification {

    public class FakeDictionaryConverter<T, U> : JsonConverter<MutableKeysDictionary<T, U>> {
        public override void WriteJson(JsonWriter writer, MutableKeysDictionary<T, U>? value, JsonSerializer serializer) {
            serializer.Serialize(writer, value?.ToDictionary());
        }

        #nullable disable
        public override MutableKeysDictionary<T, U> ReadJson(JsonReader reader, Type objectType, MutableKeysDictionary<T, U>? existingValue, bool hasExistingValue, JsonSerializer serializer) {
            var dict = serializer.Deserialize<Dictionary<T, U>>(reader);
            var fakeDict = new MutableKeysDictionary<T, U>();
            foreach (var kvp in dict) {
                fakeDict[kvp.Key] = kvp.Value;
            }
            return fakeDict;
        }
        #nullable enable
    }

    [JsonConverter(typeof(FakeDictionaryConverter<IDPrimaryMark, List<IDTraceMark>>))]
    public class MutableKeysDictionary<T, U> : IEnumerable<KeyValuePair<T, U>> {

        // --- VARIABLES ---

        // - value holder (dictionary-like) -

        private readonly List<(T Key, U Value)> Entries = new();

        // - Value Exposure and Overring -

        public IEnumerable<T> Keys => Entries.Select(e => e.Key);

        public IEnumerable<U> Values => Entries.Select(e => e.Value);

        public int Count {
            get => Entries.Count;
        }
        
        // - Indexation -

        public U this[T key] {
            get {
                var entry = Entries.Find(e => EqualityComparer<T>.Default.Equals(e.Key, key));
                if (entry.Equals(default))
                    throw new KeyNotFoundException();
                return entry.Value;
            }
            set {
                for (int i = 0; i < Entries.Count; i++) {
                    if (EqualityComparer<T>.Default.Equals(Entries[i].Key, key)) {
                        Entries[i] = (key, value);
                        return;
                    }
                }
                Entries.Add((key, value));
            }
        }

        // --- METHODS ---

        public bool ContainsKey(T key) =>
            Entries.Exists(e => EqualityComparer<T>.Default.Equals(e.Key, key));

        public bool ContainsValue(U value) =>
            Entries.Any(e => EqualityComparer<U>.Default.Equals(e.Value, value));

        public bool Remove(T key) {
            int index = Entries.FindIndex(e => EqualityComparer<T>.Default.Equals(e.Key, key));
            if (index < 0) return false;
            Entries.RemoveAt(index);
            return true;
        }

        // - Json Serialization Support -

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<KeyValuePair<T, U>> GetEnumerator() {
            foreach (var (Key, Value) in Entries) {
                yield return new KeyValuePair<T, U>(Key, Value);
            }
        }

        #nullable disable
        public Dictionary<T, U> ToDictionary() {
            var dict = new Dictionary<T, U>();
            foreach (var (key, value) in Entries) {
                dict[key] = value;
            }
            return dict;
        }
        #nullable enable
    }
}
