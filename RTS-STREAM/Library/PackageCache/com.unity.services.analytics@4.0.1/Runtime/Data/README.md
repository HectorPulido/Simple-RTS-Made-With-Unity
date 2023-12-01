# Data

These files are about holding and serializing the data that will be pushed to
the UA2 backend.

The main theme of these files is to take in Data in the basic form for UA2
(double, int, string, etc) and store them, until the API requests the data
to be Serialized.

## Standard Events

Files with the prefix `Std` are about generating data for the Standard Events.

## Testing 

Ideally we want these files to be 'dumb' so that we can test the generation easily,
and in isolation. We do this by letting the API layer handle the smart stuff.
So its up to the API layer to correctly set things like user country and platform.
