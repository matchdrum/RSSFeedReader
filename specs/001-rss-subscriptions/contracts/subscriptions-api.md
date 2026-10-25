# Subscriptions API Contract

The API supports only adding and listing subscriptions. It does not fetch,
parse, or verify feed content.

## `GET /api/subscriptions`

Returns all accepted entries in insertion order.

**Success: `200 OK`**

```json
[
  { "url": "https://example.com/feed.xml" }
]
```

Each accepted URL appears at most once.

## `POST /api/subscriptions`

Adds one URL to the in-memory collection.

**Request: `application/json`**

```json
{ "url": "https://example.com/feed.xml" }
```

The submitted value is trimmed of surrounding whitespace, then validated as an
absolute HTTP or HTTPS URL with a non-empty host and no more than 2048
characters. The 2048-character maximum is an explicit MVP input bound.
Validation does not make a network request. Before adding, the API compares
the trimmed URL string exactly against existing entries.

**Success: `200 OK`**

```json
{ "url": "https://example.com/feed.xml" }
```

**Duplicate: `409 Conflict`**

Returned when an existing subscription has the same trimmed URL string. The
response identifies the duplicate and leaves the collection unchanged.

**Invalid request: `400 Bad Request`**

Used for a missing/null URL, an empty or whitespace-only value, a value longer
than 2048 characters, a malformed/non-absolute URL, a URL without a host, or a
URL using a scheme other than HTTP or HTTPS. The response identifies the
invalid input without changing the collection. Use the application's standard
validation error representation.
