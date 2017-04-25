#define SEG_D1 4
#define SEG_D2 7
#define SEG_D3 8
#define SEG_D4 A0

#define SEG_A 5
#define SEG_B 9
#define SEG_C A2
#define SEG_D A4
#define SEG_E A5
#define SEG_F 6
#define SEG_G A1
#define SEG_DP A3

const byte nums[10] = {0b11111100, 0b01100000, 0b11011010, 0b11110010, 0b01100110, 0b10110110, 0b10111110, 0b11100000, 0b11111110, 0b11110110};

byte h = 21;
byte m = 01;
byte s = 0;

void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
  
  pinMode(SEG_D1, OUTPUT);
  pinMode(SEG_D2, OUTPUT);
  pinMode(SEG_D3, OUTPUT);
  pinMode(SEG_D4, OUTPUT);
  pinMode(SEG_A, OUTPUT);
  pinMode(SEG_B, OUTPUT);
  pinMode(SEG_C, OUTPUT);
  pinMode(SEG_D, OUTPUT);
  pinMode(SEG_E, OUTPUT);
  pinMode(SEG_F, OUTPUT);
  pinMode(SEG_G, OUTPUT);
  pinMode(SEG_DP, OUTPUT);
  digitalWrite(SEG_D1, 1);
  digitalWrite(SEG_D2, 1);
  digitalWrite(SEG_D3, 1);
  digitalWrite(SEG_D4, 1);

  TCCR1A = 0; // Start
  TCCR1B = 5; // CLK / 1024 (15625 Hz)
  TCNT1 = 0; // Count = 0
  OCR1A = (uint16_t)15625 * 4;
  TIFR1 = 2; // Reset OC flag
  TCCR1B = 0b00001101;
}

void loop() {
  // put your main code here, to run repeatedly:

  byte command = 0;
  while ((Serial.available() > 0) && (command < 201))
  {
    command = Serial.read();
  }
  
  write_digit(nums[h / 10], 1);
  delay(1);
  write_digit(nums[h % 10] | 1 , 2);
  delay(1);
  write_digit(nums[m / 10], 3);
  delay(1);
  write_digit(nums[m % 10], 4);
  delay(1);

  switch(command)
  {
    case 254:
    {
      if (Serial.available() > 2)
      {
        h = Serial.read();
        m = Serial.read();
        s = Serial.read();
      }
      break;
    }
  }

  if(TIFR1 & 2)
  {
    TCNT1 = 0;
    TIFR1 = 2;
    s = s + 4;
    if(s > 59)
    {
      s = 0;
      m = m + 1;
      if(m > 59)
      {
        m = 0;
        h = h + 1;
        if(h > 23)
        {
          h = 0;
        }
      }
    }
  }
}

void write_digit(byte data, byte digit)
{
  digitalWrite(SEG_D1, 1);
  digitalWrite(SEG_D2, 1);
  digitalWrite(SEG_D3, 1);
  digitalWrite(SEG_D4, 1);
  write_seg(data);
  switch (digit)
  {
    case 1:
      {
        digitalWrite(SEG_D1, 0);
        break;
      }
    case 2:
      {
        digitalWrite(SEG_D2, 0);
        break;
      }
    case 3:
      {
        digitalWrite(SEG_D3, 0);
        break;
      }
    case 4:
      {
        digitalWrite(SEG_D4, 0);
        break;
      }
  }
  //write_seg(data);
}

void write_seg(byte data)
{
  digitalWrite(SEG_A, data & 0b10000000);
  digitalWrite(SEG_B, data & 0b01000000);
  digitalWrite(SEG_C, data & 0b00100000);
  digitalWrite(SEG_D, data & 0b00010000);
  digitalWrite(SEG_E, data & 0b00001000);
  digitalWrite(SEG_F, data & 0b00000100);
  digitalWrite(SEG_G, data & 0b00000010);
  digitalWrite(SEG_DP, data & 0b00000001);
}

