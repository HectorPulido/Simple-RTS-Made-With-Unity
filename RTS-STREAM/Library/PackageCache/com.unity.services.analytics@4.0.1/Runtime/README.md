# Runtime Documentation

This document only outlines some of the general runtime themes, this is not an
in-depth guide.

## General Design

The general structure is the public API sits on top, and orchestrates the
interactions with the components below. Each component sits in its own directory.

```plaintext
     ---------------------------------------------
     |                                           |
     |            Analytics Runtime API          |
     |                                           |
     |------||-------------||------------||------|
     |              |              |             |
     |   Data Gen   |   Platform   |   Runtime   |
     |              |              |             |
     ---------------------------------------------
```

- `./*.cs` the public API, this brings together the elements below. Nothing under this should be public to the calling code.
- `./Data/*.cs` files handling the creating and storing of event data.
- `./Platform/*.cs` files that handle various platform information.
- `./Runtime/*.cs` files that contain runtime logic.

## General Themes

### Lazy Init

The user is free to push event data into the SDK even if it has not been
initialized, this is to allow data to be recorded that might exist before
initialization, or if the network is currently inactive.

The data will be stored in `Data/Buffer.cs`, and when initialized the heartbeat
will start to flush data out of the buffer.

### Event Data

The path to generate event data is as follows.

- User calls an API entry point on `Events.cs`.
- Standard events are generated in the `Data/Generator.cs`.
- This information is pushed into `Data/Buffer.cs`.
- Heartbeat then flushes out the data and sends to the backend.

### Heartbeat

Periodically the heartbeat will flush data out of the buffer, but this is only
happens after the SDK has been initialized.

- Heartbeat will callback periodically.
- If data is found in `Data/Buffer.cs` it will be serialized to JSON and sent

