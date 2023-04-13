using System;
using System.Collections;
using System.Collections.Generic;


namespace ElevatorTrip
{
    public class Request
    {

        public int currentFloor;
        public int desiredFloor;
        public Direction direction;
        public Location location;

        public Request(int currentFloor, int desiredFloor, Direction direction, Location location)
        {
            this.currentFloor = currentFloor;
            this.desiredFloor = desiredFloor;
            this.direction = direction;
            this.location = location;
        }


        public int GetCurrentFloor()
        {
            return this.currentFloor;
        }

        public int GetDesiredFloor()
        {
            return this.desiredFloor;
        }
    }
    public enum Direction
    {
        UP,
        DOWN,
        IDLE
    }
    public enum Location
    {
        INSIDE_ELEVATOR,
        OUTSIDE_ELEVATOR
    }

    public class Elevator
    {

        public int currentFloor;

        Direction direction;

        Queue myQ = new Queue();


        //PriorityQueue<int> p = new PriorityQueue<int>();

        private PriorityQueue<Request, int> upQueue;
        private PriorityQueue<Request, int> downQueue;
        public Elevator(int currentFloor)
        {
            this.currentFloor = currentFloor;
            this.direction = Direction.IDLE;

            // use default, which is a min heap
            upQueue = new PriorityQueue<Request, int>(Comparer<int>.Create((x, y) => x-y));
           
            // use max heap
            downQueue = new PriorityQueue<Request, int>(Comparer<int>.Create((x, y) => y - x));

        }

        public void sendUpRequest(Request upRequest)
        {
            //  If the request is sent from out side of the elevator,
            //  we need to stop at the current floor of the requester
            //  to pick him up, and then go the the desired floor.
            if ((upRequest.location == Location.OUTSIDE_ELEVATOR))
            {
                //  Go pick up the requester who is outside of the elevator
                this.upQueue.Enqueue(new Request(upRequest.currentFloor, upRequest.desiredFloor, Direction.UP, Location.OUTSIDE_ELEVATOR), upRequest.currentFloor - upRequest.desiredFloor);
                Console.WriteLine(("Append up request going to floor "
                                + (upRequest.currentFloor + ".")));
            }

            //  Go to the desired floor
            this.upQueue.Enqueue(upRequest,upRequest.desiredFloor - upRequest.currentFloor);
            Console.WriteLine(("Append up request going to floor "
                            + (upRequest.desiredFloor + ".")));
        }

        public void sendDownRequest(Request downRequest)
        {
            //  Similar to the sendUpRequest logic
            if ((downRequest.location == Location.OUTSIDE_ELEVATOR))
            {
                this.downQueue.Enqueue(new Request(downRequest.desiredFloor, downRequest.currentFloor, Direction.DOWN, Location.OUTSIDE_ELEVATOR), downRequest.currentFloor - downRequest.desiredFloor);
                Console.WriteLine(("Append down request going to floor "
                                + (downRequest.currentFloor + ".")));
            }

            //  Go to the desired floor
            this.downQueue.Enqueue(downRequest, downRequest.desiredFloor - downRequest.currentFloor);
            Console.WriteLine(("Append down request going to floor "
                            + (downRequest.desiredFloor + ".")));
        }

        public void run()
        {
            while ((this.upQueue.Count != 0)
                        || (this.downQueue.Count != 0))
            {
                this.processRequests();
            }

            Console.WriteLine("Finished all requests.");
            this.direction = Direction.IDLE;
        }

        private void processRequests()
        {
            
            //Assuming UP direction takes prioroty
            if (((this.direction == Direction.UP)
                        || (this.direction == Direction.IDLE)))
            {
                this.processUpRequest();
                this.processDownRequest();
            }
            else
            {
                this.processDownRequest();
                this.processUpRequest();
            }

        }

        private void processUpRequest()
        {
            while (this.upQueue.Count != 0)
            {
                Request upRequest = this.upQueue.Dequeue();
                //  Communicate with hardware
                this.currentFloor = upRequest.desiredFloor;
                Console.WriteLine(("Processing up requests. Elevator stopped at floor "
                                + (this.currentFloor + ".")));
            }

            if (this.downQueue.Count != 0)
            {
                this.direction = Direction.DOWN;
            }
            else
            {
                this.direction = Direction.IDLE;
            }

        }

        private void processDownRequest()
        {
            while (this.downQueue.Count != 0)
            {
                Request downRequest = this.downQueue.Dequeue();
                //  Communicate with hardware
                this.currentFloor = downRequest.desiredFloor;
                Console.WriteLine(("Processing down requests. Elevator stopped at floor "
                                + (this.currentFloor + ".")));
            }

            if (this.upQueue.Count != 0)
            {
                this.direction = Direction.UP;
            }
            else
            {
                this.direction = Direction.IDLE;
            }

        }
    }

    public class Program
    {
        public static void Main(String[] args)
        {
            Elevator elevator = new Elevator(0);
            Request upRequest1 = new Request(elevator.currentFloor, 5, Direction.UP, Location.INSIDE_ELEVATOR);
            Request upRequest2 = new Request(elevator.currentFloor, 3, Direction.UP, Location.INSIDE_ELEVATOR);
            Request downRequest1 = new Request(elevator.currentFloor, 1, Direction.DOWN, Location.INSIDE_ELEVATOR);
            Request downRequest2 = new Request(elevator.currentFloor, 2, Direction.DOWN, Location.INSIDE_ELEVATOR);
            Request downRequest3 = new Request(4, 0, Direction.DOWN, Location.OUTSIDE_ELEVATOR);
            //  Two people inside of the elevator pressed button to go up to floor 5 and 3.
            elevator.sendUpRequest(upRequest1);
            elevator.sendUpRequest(upRequest2);
            //  One person outside of the elevator at floor 4 pressed button to go down to floor 0
            elevator.sendDownRequest(downRequest3);
            //  Two person inside of the elevator pressed button to go down to floor 1 and 2.
            elevator.sendDownRequest(downRequest1);
            elevator.sendDownRequest(downRequest2);
            elevator.run();
        }
    }
}
