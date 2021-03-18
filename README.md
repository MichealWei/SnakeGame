# SnakeGame

Sorry, I don't know go lang yet, so I implement this task in C#. And yet, I feel it wasn't an quick job. It took me about an hour to figure out the requirements, yet I still do some
deviation from the requirement list. I do not implement a 2d int array explicitly, since when I do model the game state as a 2d array, it give me lots of headache when I try to update
the game. Neither do I use snake head and snake length, instead I use a list, the first element is the cordinate of the head, the other elements are for the tail.

It took me about two hours to finish the core logic, most time is spend in finding different ways of printing the game and update it, the nested for loop is troublesome. And It took 
about another two hours to calibrate the UI. The UI is still fragile, but I think it's enough for demo.

And to make the code more clear, with better bad input handling, I use function composition to connect the logic together.
