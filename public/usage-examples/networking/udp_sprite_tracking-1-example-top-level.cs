using SplashKitSDK;
using static SplashKitSDK.SplashKit;

// Open a window so the UDP message flow can be seen live on screen.
OpenWindow("UDP Sprite Tracking", 800, 600);

// Create the bitmap in code so the example works without depending on an external image file.
Bitmap trackerBitmap = CreateBitmap("tracker", 60, 60);

// Fill the bitmap so the sprite is clearly visible during the recording.
ClearBitmap(trackerBitmap, ColorBlack());

// Create a sprite from the bitmap because the task is about sending sprite position data.
Sprite trackerSprite = CreateSprite("tracker_sprite", trackerBitmap);

// Start the sprite away from the text so both the movement and messages stay easy to read.
MoveSpriteTo(trackerSprite, 100, 250);

// Create a UDP server to act like the hub that receives the position packets.
ServerSocket hubServer = CreateServer("hub", 5001, ConnectionType.UDP);

// Open a UDP connection that acts like the sender in the same demo.
Connection senderConnection = OpenConnection("sender", "127.0.0.1", 5001, ConnectionType.UDP);

// Keep the most recent received message so the user can see the network result on screen.
string latestReceived = "No packet received yet";

// Keep the current sent message so the sent and received values can be compared visually.
string sentMessage = "";

// Use one speed variable so direction can be flipped cleanly at each edge.
float speed = 3.0f;

while (!QuitRequested())
{
    // Process events each frame so the window stays responsive.
    ProcessEvents();

    // Reverse the direction at the right side so the sprite keeps moving back and forth.
    if (SpriteX(trackerSprite) >= 650)
    {
        speed = -3.0f;
    }
    // Reverse the direction at the left side for the same looping movement.
    else if (SpriteX(trackerSprite) <= 50)
    {
        speed = 3.0f;
    }

    // Move the sprite using a vector because SplashKit sprite movement uses vector-based motion.
    MoveSprite(trackerSprite, VectorTo(speed, 0));

    // Build the UDP packet from the live sprite position so the message matches the moving object.
    sentMessage = "POS:" + (int)SpriteX(trackerSprite) + "," + (int)SpriteY(trackerSprite);

    // Send the current position so the example demonstrates live UDP data streaming.
    SendMessageTo(sentMessage, senderConnection);

    // Check network activity so sent packets can be received and read in the same loop.
    CheckNetworkActivity();

    // Read the newest message only when one is available so the display stays stable.
    if (HasMessages(hubServer))
    {
        latestReceived = ReadMessageData(hubServer);
    }

    // Clear the screen so each frame redraws cleanly without visual trails.
    ClearScreen(ColorWhite());

    // Draw the sprite so the moving object being tracked is visible.
    DrawSprite(trackerSprite);

    // Draw a title so the purpose of the window is immediately clear.
    DrawText("UDP Sprite Tracking", ColorBlack(), 20, 20);

    // Draw the sent packet so the outgoing data is visible each frame.
    DrawText("Sent: " + sentMessage, ColorBlue(), 20, 60);

    // Draw the received packet so the loopback UDP result can be compared easily.
    DrawText("Received: " + latestReceived, ColorRed(), 20, 100);

    // Add one short explanation so the demo still makes sense in a GIF without narration.
    DrawText("Sprite position is sent with UDP and read back by the hub.", ColorDarkGreen(), 20, 140);

    // Refresh at 60 FPS so movement and text updates look smooth in the recording.
    RefreshScreen(60);
}

// Close the client connection so the example exits cleanly.
CloseConnection(senderConnection);

// Close the server so the used port is released properly.
CloseServer(hubServer);