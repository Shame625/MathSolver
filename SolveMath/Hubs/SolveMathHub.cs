using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SignalR.Hubs
{
    public static class CurrentGame
    {
        public static Equation currentEquation;

        public static Equation GetCurrentEquation()
        {
            if (currentEquation == null)
            {
                return StartNewGame();
            }

            return currentEquation;
        }

        public static Equation StartNewGame()
        {
            currentEquation = new Equation();
            return currentEquation;
        }

        public static bool IsCorrect(int input)
        {
            if (input == currentEquation.GetResult())
                return true;
            return false;
        }
    }
    public class SolveMath : Hub
    {
        public async Task GetCurrentGame()
        {
            await Clients.Caller.SendAsync("Equation", CurrentGame.GetCurrentEquation());
        }

        public async Task SendAnswer(int message)
        {
            if(CurrentGame.IsCorrect(message))
            {
                CurrentGame.currentEquation.hasWinner = true;
                CountdownToNewGame();
            }
            else if(CurrentGame.currentEquation.hasWinner)
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "MSG", "Game already finished!");
            }
            else
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "MSG", "Wrong, try again.");
            }
        }

        private async void CountdownToNewGame()
        {
            await Clients.All.SendAsync("ReceiveMessage", "GAME_RESTARTING", "WE HAVE A WINNER, CORRECT ANSWER WAS: " + CurrentGame.currentEquation.GetResult());
            int seconds = 5;
            for(;seconds >= 0; seconds--)
            {
                await Clients.All.SendAsync("ReceiveMessage", "GAME", "SERVER: " + seconds);
                Thread.Sleep(1000);
            }
            CurrentGame.StartNewGame();

            await Clients.All.SendAsync("Equation", CurrentGame.GetCurrentEquation());
        }
    }

    public class Equation
    {
        Random rnd = new Random();

        public Equation()
        {
            x = rnd.Next(1, 100);
            y = rnd.Next(1, 100);
            result = x + y;
            hasWinner = false;
        }

        public int x { get; }
        public int y { get; }
        private int result;
        public bool hasWinner;

        public int GetResult()
        {
            return result;
        }
    }


}