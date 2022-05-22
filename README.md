# wizards-duel-ar

Wizard’s Duel AR is a cross-platform, multiplayer AR brawler for players with ARKit- and ARCore-supported smartphones. Two or more players engage in real-time combat by blasting each other with magic and defending themselves with shields. There are no win conditions or rules. Players can host new duels and join or spectate existing duels.

![](https://thumbs.gfycat.com/FaroffFrenchGelada-size_restricted.gif)

This is a prototype I developed showcasing what you can do combining augmented reality with real-time multiplayer networking. 

> The project is not currently supported. 

## Stack

- Unity
- [GameSparks](https://docs.gamesparks.com/tutorials/real-time-services/) for real-time multiplayer networking
- [Google Cloud Anchors](https://developers.google.com/ar/develop/java/cloud-anchors/overview-android) for shared augmented reality experiences

## How It Works

**Google Cloud Anchors**

During I/O ‘18, Google presented a crafty solution called Google Cloud Anchors, which allows users to host an anchor to the cloud that others can then resolve. What this means is that users can share a spatial mapping of their surrounding environment with each other, and Google helps put together a common frame of reference via an anchor—the cloud anchor.

![](https://media.giphy.com/media/1xpBModVoey49S9FvG/giphy.gif)

With this same cloud anchor situated in the environment on every player’s camera screen, every player’s game has the same reference point from which to share their phone’s relative position and rotation.

Google provides an SDK for Unity, allowing developers to create cross-platform applications. I leveraged the `CloudAnchorController` script used in the `CloudAnchor` scene in the `GoogleARCore` package.

Before building out the game from Unity, make sure the project is set up to use Cloud Anchors. Follow the instructions on their webpage (iOS, Android) to configure a few project settings.

**GameSparks real-time services**

Powered by AWS, GameSparks is a full-on backend-as-a service targeted specifically for online multiplayer games. They provide low-latency multiplayer networking solutions that are perfect for real-time synchronization of virtual content between players.

GameSparks also provides an SDK for Unity and lots of documentation and examples for its real-time services.

## Gameplay

Wizard’s Duel AR is a cross-platform, multiplayer AR brawler for players with ARKit- and ARCore-supported smartphones. Two or more players engage in real-time combat by blasting each other with magic and defending themselves with shields. There are no win conditions or rules. Players can host new duels and join or spectate existing duels.

![](https://thumbs.gfycat.com/RemorsefulContentArctichare-size_restricted.gif)

**Camera-tracking wands**

The wand is a visual representation of the position and rotation of each player’s smartphone shared live with all other players. Mobile AR is not without its faults, so how far the wand is from the actual player’s phone gives you an idea of the discrepancy that happens due to drifting.

![](https://thumbs.gfycat.com/WhichLightAracari-size_restricted.gif)

Additionally, I added a widget that floats above the phone’s theoretical position and displays the player’s username. It’s a visual aide just like the wand, and it can be customized to share other relevant information, such as health, team, and score. The widget has a billboard script, so it will always face your camera.

**Shields**

I introduced shields not only as a defensive mechanic but also as a cooperative mechanic that encourages players to physically interact with each other. For example, you could have a version of this game with teams where the win condition is to reduce your opponent’s health. Low-health players could take cover behind the shields of their teammates as they recover.

![](https://thumbs.gfycat.com/DeepForkedArachnid-size_restricted.gif)

**Invisible colliders**

As fun it is to blast your opponents, it wouldn’t be very satisfying if the projectiles kept flying through their bodies.

I added capsule colliders that are positioned roughly within the vicinity of the player’s body, so that when you shoot another player and it looks like you’ve hit them, the projectile collides with the collider and activates an explosion effect.

**Drop in & drop out**

Lastly, the game is configured to allow players to jump in and out of the game and even spectate if they want to. AR experiences are extensions of the real world, so any public activity should be open to bystanders.

## Game Flow

Players log-in with a username and password. Players can input any random username and password to be authenticated.

One player selects the option to Host a duel. An AR session is initiated and the player’s camera scans the environment. Once a plane has been created, the hosting player can tap on the screen to place a cloud anchor on the plane and submit it to the Google servers. Once the anchor is successfully processed, other players can join this AR session by resolving the cloud anchor. The game does not start until at least one other player has entered the duel.

All other players who are joining the duel select the option to Join. The players must input the room number of the duel and the IP address of the host player — this information is available in the top right corner of the host player’s screen. Once the required information has been inputted, the camera scans the environment to resolve the cloud anchor. Once the anchor has been successfully resolved, the player joins the AR session created by the host player and the game starts.

When the game starts, all players in the AR session can immediately begin dueling.

## Additional Thoughts

Playing AR games on mobile at a 1:1 scale with the real world is quite awkward. Trying to dodge a projectile that you can only see through your phone screen creates an immediate disconnect from reality. That’s why most mobile AR applications either scale the virtual content down to fit the space in front of you or are planted down at a set location that doesn’t require you to look around.

![41BC7815-4E81-4BAA-BBA3-C082BD4D38D1](https://user-images.githubusercontent.com/13254616/169715511-60840a6f-d04c-40cd-ac33-bceb171515f1.gif)

That’s why the future of AR is not in mobile but in glasses to create a fully immersive experience.

If you plan on making a game similar to this project, I recommend trying the following ideas:

- Spawn items and equipment on the floor or wall that you can position yourself around rather than attaching them in front of the phone. For obvious reasons, you just can’t see anything if you put, let’s say, a shield in front.
- Draw symbols on the floor below an opponent to cast spells. Since this uses raycasting, players can feel their touch actions on the phone screen be transposed directly onto the real world.
- Be creative with projectiles. Your eyes have no depth perception on the phone screen because everything is scaled down. You have no way of gauging where and how far you’re shooting — try it yourself by turning off the cross-hairs in a first-person shooter game. However, there are ways to address these challenges. You could try, for example, line renderers instead of particles for the projectile trail because the line renderer visibly reduces in size the farther it travels. You could also have a more concrete object, like a ball or rocket.
