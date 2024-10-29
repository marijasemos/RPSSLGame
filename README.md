# RPSSL Game - Rock, Paper, Scissors, Lizard, Spock

## Description
RPSSL Game is an implementation of the popular game "Rock, Paper, Scissors, Lizard, Spock". It extends the classic "Rock, Paper, Scissors" game by adding two new moves: Lizard and Spock.
This project is built using microservices in C# for game mechanics and user management, and a React frontend for a seamless user interface.

The game logic is containerized and managed using Docker Compose, allowing for easy deployment and scaling.

## Technologies Used
- C# (.NET Core) for microservices
- React for frontend
- Docker & Docker Compose for container orchestration

## Installation and Running

### Prerequisites
- .NET Core SDK installed
- Node.js and npm installed
- Docker Desktop  installed

### Steps to Run

1. Clone the repository:
    git clone https://github.com/username/RPSSLGame.git
   
3. Clone the repository and place in the previous one
 git clone https://github.com/marijasemos/rpssl.git

RPSSLGame/
│
├── RPSSL.ChoiceService/     
├── RPSSL.PlayService/      
├── rpssl/          
├── docker-compose.yml  
└── README.md          


5. Build and run the microservices using Docker Compose:
    docker-compose up --build

6. Navigate to the `react-app` directory and install dependencies:
    cd rpssl
    npm install

7. Start the React application
    npm run start

8. Open your web browser and go to `http://localhost:3000` to access the game or http://rpssl.localhost/.

##  Play Rules
    - Scissors cuts Paper
    - Paper covers Rock
    - Rock crushes Lizard
    - Lizard poisons Spock
    - Spock smashes Scissors
    - Scissors decapitates Lizard
    - Lizard eats Paper
    - Paper disproves Spock
    - Spock vaporizes Rock
    - Rock crushes Scissors
 Enjoy and see who wins!

## How to Play 

The RPSLS game offers two modes: playing against the computer or playing against another player.


## How to Playing Against the Computer
1.Choose one of the five available options: Rock, Paper, Scissors, Lizard, or Spock.
2.Once you make your selection, the computer will randomly generate its choice.
3.The game will determine the winner based on the rules of RPSLS.


## How to Playing Against Another Player

1.One player creates a new game session and receives a unique game code.
2.The second player joins the game by entering the game code provided by the first player.
3.The game begins once both players are connected, and they can continue playing rounds until one player decides to end the session.
4.The game will automatically end if a player does not make a move within 20 seconds.
Enjoy the game!

## Project Structure
- RPSSL.GameService - Contains the game logic microservice.
- RPSSL.ChoiceService - Manages choices.
- rpssl - The frontend of the game built using React.
- docker-compose.yml - Docker Compose configuration file for managing containers.

## Contributing
Contributions are welcome! Please fork the repository, make your changes, and submit a pull request. Ensure that your code follows the established code style and has been properly tested.

## Authors
Marija Milosevic
