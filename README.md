# BlockVotes

BlockVotes is a simplified blockchain voting application built with ASP.NET Core that demonstrates the core concepts of blockchain technology through a voting mechanism.

## Overview

This application simulates a blockchain-based voting system where:

- Users can cast votes for candidates
- Votes are collected in a pending pool
- Votes are "mined" into blocks and added to the blockchain
- The entire voting history is immutable and transparent

## Features

- **Vote Casting**: Users can submit their ID and candidate choice
- **Pending Vote Pool**: Votes are held in a pending state until mined
- **Mining Process**: Simulates the blockchain mining process with proof-of-work
- **Block Explorer**: View the entire blockchain with block details
- **Vote Aggregation**: Votes are grouped by candidate with totals displayed

## Technical Implementation

The application consists of these key components:

- **Block**: Represents a container for votes with cryptographic properties
  - Contains a list of votes, timestamp, hash, previous hash, and nonce
  - Implements proof-of-work mining with adjustable difficulty

- **Blockchain**: Manages the chain of blocks and pending votes
  - Maintains the integrity of the blockchain through hash validation
  - Handles adding votes to pending pool and mining new blocks

- **Vote**: Simple data structure representing a single vote
  - Contains voter ID, candidate name, and timestamp

## How It Works

1. Users submit votes through the web interface
2. Votes are initially stored in the pending votes pool
3. The mining process creates a new block containing the pending votes
4. The new block is added to the blockchain after successful mining
5. The blockchain maintains integrity by linking each block to the previous one through hashes

## Getting Started

### Prerequisites

- .NET 9 SDK
- SQL Server or SQLite for database storage

### Running the Application

1. Clone the repository
2. Navigate to the project directory
3. Run `dotnet run`
4. Access the application at `https://localhost:7000` or `http://localhost:5000`

### Database Setup

Before running the application, you need to set up the database:

1. Update the connection string in `appsettings.json` to point to your database
2. Open a terminal in the project directory
3. Run the following commands to create and apply migrations:

```cmd
> dotnet ef migrations add InitialCreate
> dotnet ef database update
```

## Usage
- **Create an Account**: Register a new user account to start voting
- **Login**: Use your credentials to log in
- **Voting**: Navigate to the voting page to cast your vote
- **Candidate Selection**: Choose from the list of candidates and submit your vote
- **Viewing Pending Votes**: Navigate to the Pending page to see votes waiting to be mined
- **Close Voting**: Click the "Close Voting" button to mine the pending votes into a new block
- **Check Blockchain**: View the entire blockchain and verify the integrity of the votes
- **View Results**: Check the aggregated vote counts for each candidate

## Educational Purpose

This application is intended as an educational tool to demonstrate blockchain concepts including:

- Immutable ledgers
- Cryptographic hashing
- Proof-of-work consensus
- Distributed data storage

Note: This is a simplified implementation for demonstration purposes and lacks many features required for a production voting system, such as proper security, and distributed consensus.
