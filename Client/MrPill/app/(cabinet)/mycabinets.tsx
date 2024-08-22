import React, { useEffect, useState } from 'react';
import { router } from 'expo-router';
import { ThemedText } from '@/components/ThemedText';
import { View, StyleSheet } from 'react-native';
import { AppHomeButton } from "@/components/AppHomeButton";
import { MrPillLogo } from '@/components/MrPillLogo';
import { strFC } from "@/components/strFC";
import ParallaxScrollView from "@/components/ParallaxScrollView";
import DataHandler from "@/DataHandler";
import { Pressable } from 'react-native';
import axios from 'axios';

type Cabinet = {
    userId: number;
    reminderTime: string;
    message?: string;
    isRecurring: boolean;
    recurrenceInterval?: string;
};

function getFamilyEmoji() {
    const familyEmojis = ["👪","👨‍👩‍👦","👨‍👩‍👧","👨‍👩‍👧‍👦","👨‍👩‍👦‍👦","👨‍👩‍👧‍👧","👨‍👨‍👦","👨‍👨‍👧","👩‍👩‍👧‍👧","👩‍👩‍👦‍👦","👩‍👩‍👧‍👦","👩‍👩‍👧","👩‍👩‍👦","👨‍👨‍👧‍👧","👨‍👨‍👦‍👦","👨‍👨‍👧‍👦","👨‍👧‍👦","👨‍👧","👨‍👦","👩‍👧‍👧","👩‍👦‍👦","👩‍👧‍👦","👩‍👧","👩‍👦","👨‍👦‍👦","👨‍👧‍👧"];
    return familyEmojis[Math.floor(Math.random() * 26)];
}

const backgroundColorLight = "#71bfe9"
const backgroundColorMain = "#e6c8c8"
const borderColor = "#005a27"

const MyCabinets: React.FC = () => {

    const user = DataHandler.getUser()
    const dateTime = new Date();
    const [myCabinets, setMyCabinets] = useState<Cabinet[]>([]);
    //const [ownerId, setOwnerId] = useState<number>(0);
    //const [dropdownVisible, setDropdownVisible] = useState(false);
  
    useEffect(() => {

        const fetchCabinets = async () => {
            const request = {
                method: 'post',
                url: "http://10.0.2.2:5181/Mr-Pill/user/cabinets",
                headers: { "Content-Type": "application/json" }, 
                data: {
                "UserToken": user.Token,
                "PrivacyStatus": "AllMedications",
            }
          }
    
          try {
            const response = await axios(request);
            console.log(response.data);
            setMyCabinets(response.data);
          } catch (error) {
            console.error("Error fetching cart items:", error);
          }
        };

        fetchCabinets();
    })
    

    function renderCabinet(cabinet?: Cabinet) {

    return (
      <Pressable onPress={()=>{console.log('y')}}>
        <View style={styles.reminderBox}>
          <View style={{flexDirection: 'row'}}>
    
          <View style={[styles.plusMinusButton, {backgroundColor: "#90e665"}]}>
            <ThemedText style={[styles.plusMinusText, {paddingTop: 13.5}]}>💊</ThemedText>
          </View>

          <View style={[styles.plusMinusButton, {backgroundColor: "#90e665"}]}>
            <ThemedText style={[styles.plusMinusText, {paddingTop: 15.5}]}>{getFamilyEmoji()}</ThemedText>
          </View>
            
          <View>
            <ThemedText>ארון תרופות פנתרים</ThemedText>
            <ThemedText>בשעה 14:00 לאחר האוכל </ThemedText>
          </View>
    
          </View>
        </View>
      </Pressable>
    )
  }

  return (
    <View style={{backgroundColor: backgroundColorMain, flex: 1}}>
        <View style={{flex: 1, minHeight: 40}}>
            {MrPillLogo(1)}
        </View>
        <View style={{flex: 1, minHeight: 50,}}>
            <View style={styles.pagetop}> 
                <ThemedText style={{textAlign: 'center', fontSize: 24, textDecorationLine: 'underline', fontWeight: 'bold', marginTop: 8}}>
                    ארונות תרופות משותפים שלי:{"\n"}
                </ThemedText>
                <ParallaxScrollView backgroundColor={backgroundColorLight}>
                    {renderCabinet()}
                    {renderCabinet()}
                    {renderCabinet()}
                    {renderCabinet()}
                </ParallaxScrollView>
            </View>
        </View>
        <View style={styles.pagebottom}>
            <View style={styles.row}>
                <AppHomeButton BackgroundColor={backgroundColorLight} BorderColor={borderColor} ButtonContent={strFC("הוסף ארון חדש")} ButtonAction={()=>{router.navigate('/(cabient)/addcabinet')}}/>
            </View>
        </View>
    </View>
  );
};

const styles = StyleSheet.create({
  pagetop: {
    flex: 1,
    justifyContent: 'flex-start',
    alignItems: 'center',
    backgroundColor: backgroundColorLight,
    borderRadius: 20,
    borderWidth: 2,
    borderColor: borderColor,
    minHeight: 100,
    marginHorizontal: 15,
    padding: 5,
  },
  pagebottom: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    borderRadius: 20,
    marginHorizontal: 15,
    marginVertical: 20,
    padding: 5,
    maxHeight: 180
  },
  row: {
    flex: 1,
    minHeight: 5,
    justifyContent: 'center',
    alignItems: 'center',
    backgroundColor: backgroundColorMain,
    flexDirection: 'row',
  },
  text: {
    fontSize: 20,
    color: '#000',
  },
  reminderBox: {
    backgroundColor: 'pink',
    borderWidth: 2,
    borderColor: borderColor,
    borderRadius: 12,
    flex: 1,
    justifyContent: 'center',
    alignContent: 'center',
    paddingHorizontal: 15,
    paddingVertical: 5,
    minWidth: 300,
  },
  plusMinusButton: {
    minWidth: 50,
    minHeight: 50,
    borderRadius: 25,
    borderWidth: 2,
    borderColor: backgroundColorLight,
    justifyContent: 'center',
    alignItems: 'center',
    marginRight: 12,
  },
  plusMinusText: {
    fontSize: 30,
    fontWeight: 'bold',
    position: 'absolute',
  }
});

export default MyCabinets;
