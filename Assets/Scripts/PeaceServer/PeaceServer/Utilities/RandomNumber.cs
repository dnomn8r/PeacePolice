using System.Collections;

public class RandomNumber{

	private const uint STARTING_RANDOM_SEED_1 = 0xc023;
	private const uint STARTING_RANDOM_SEED_2 = 0x01ab;
	
	public static uint randomSeed1 = STARTING_RANDOM_SEED_1;
	public static uint randomSeed2 = STARTING_RANDOM_SEED_2;

	public static uint getRandomInt(){
		
		randomSeed1 = 36969 * (randomSeed1 & 65535) + (randomSeed1 >> 16);
		randomSeed2 = 18000 * (randomSeed2 & 65535) + (randomSeed2 >> 16);
		return (randomSeed1 << 16) + randomSeed2;  /* 32-bit result */
	}
	
	public static int Range(int min, int max){
		
		if(min >= max) return min;
		
		int rangeAmount = (int)(getRandomInt() % (uint)(max - min));
		
		return min + rangeAmount;
	}
}
