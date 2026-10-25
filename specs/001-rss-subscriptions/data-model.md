# Data Model: MVP RSS/Atom Subscription List

## Subscription Entry

Represents one URL submitted by the local user for the duration of the running
API process.

| Field | Type | Required | Rules |
|---|---|---:|---|
| `url` | string | Yes | Trim surrounding whitespace; 1–2048 characters; absolute URL with a non-empty host and `http` or `https` scheme |

### Collection behavior

- Entries remain in submission order.
- Before adding an entry, compare its trimmed URL string with the trimmed URL
  strings already in the collection using exact string equality. Reject an
  exact match and leave the collection unchanged.
- The collection begins empty when the API process starts.
- Entries are discarded when the API process stops.
- The MVP does not fetch the URL, validate feed contents, or persist data.

### State transitions

1. **Not present**: A URL has not been accepted during this process.
2. **Added**: A valid, non-duplicate submission creates a new entry at the end
   of the collection.

Invalid or duplicate submissions do not change collection state.
