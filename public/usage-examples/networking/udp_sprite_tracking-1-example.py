from splashkit import *

# Open a window so the UDP message flow can be seen live on screen.
open_window("UDP Sprite Tracking", 800, 600)

# Create the bitmap in code so the example works without depending on an external image file.
tracker_bitmap = create_bitmap("tracker", 60, 60)

# Fill the bitmap so the sprite is clearly visible during the recording.
clear_bitmap(tracker_bitmap, COLOR_BLACK)

# Create a sprite from the bitmap because the task is about sending sprite position data.
tracker_sprite = create_sprite_named("tracker_sprite", tracker_bitmap)

# Start the sprite away from the text so both the movement and messages stay easy to read.
move_sprite_to(tracker_sprite, 100, 250)

# Create a UDP server to act like the hub that receives the position packets.
hub_server = create_server_with_port_and_protocol("hub", 5001, ConnectionType.UDP)

# Open a UDP connection that acts like the sender in the same demo.
sender_connection = open_connection_with_protocol("sender", "127.0.0.1", 5001, ConnectionType.UDP)

# Keep the most recent received message so the user can see the network result on screen.
latest_received = "No packet received yet"

# Keep the current sent message so the sent and received values can be compared visually.
sent_message = ""

# Use one speed variable so direction can be flipped cleanly at each edge.
speed = 3.0

while not quit_requested():
    # Process events each frame so the window stays responsive.
    process_events()

    # Reverse the direction at the right side so the sprite keeps moving back and forth.
    if sprite_x(tracker_sprite) >= 650:
        speed = -3.0
    # Reverse the direction at the left side for the same looping movement.
    elif sprite_x(tracker_sprite) <= 50:
        speed = 3.0

    # Move the sprite using a vector because SplashKit sprite movement uses vector-based motion.
    move_sprite(tracker_sprite, vector_to(speed, 0))

    # Build the UDP packet from the live sprite position so the message matches the moving object.
    sent_message = f"POS:{int(sprite_x(tracker_sprite))},{int(sprite_y(tracker_sprite))}"

    # Send the current position so the example demonstrates live UDP data streaming.
    send_message_to_connection(sent_message, sender_connection)

    # Check network activity so sent packets can be received and read in the same loop.
    check_network_activity()

    # Read the newest message only when one is available so the display stays stable.
    if has_messages(hub_server):
        latest_received = read_message_data_from_server(hub_server)

    # Clear the screen so each frame redraws cleanly without visual trails.
    clear_screen(COLOR_WHITE)

    # Draw the sprite so the moving object being tracked is visible.
    draw_sprite(tracker_sprite)

    # Draw a title so the purpose of the window is immediately clear.
    draw_text("UDP Sprite Tracking", COLOR_BLACK, 20, 20)

    # Draw the sent packet so the outgoing data is visible each frame.
    draw_text("Sent: " + sent_message, COLOR_BLUE, 20, 60)

    # Draw the received packet so the loopback UDP result can be compared easily.
    draw_text("Received: " + latest_received, COLOR_RED, 20, 100)

    # Add one short explanation so the demo still makes sense in a GIF without narration.
    draw_text("Sprite position is sent with UDP and read back by the hub.", COLOR_DARK_GREEN, 20, 140)

    # Refresh at 60 FPS so movement and text updates look smooth in the recording.
    refresh_screen(60)

# Close the client connection so the example exits cleanly.
close_connection(sender_connection)

# Close the server so the used port is released properly.
close_server(hub_server)