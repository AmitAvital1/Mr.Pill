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
  id: number,
  medicineCabinetName: string,
  creatorId: number,
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

    let myCabinets: [Cabinet];

    const [renderedCabinets, setRenderedCabinets] = React.useState<any>([]);
    const [isRequestSent, setIsRequestSent] = React.useState<boolean>(false);
  
    useEffect(() => {
      
      if(isRequestSent) return;
      setIsRequestSent(true);

      function renderCabinet(cabinet: Cabinet, isOwnedByMe?: boolean) {
        
        return (
          <Pressable onPress={()=>{console.log('y')}}>
  
            <View style={styles.reminderBox}>
              <View style={{alignItems: 'center', flexDirection: 'row'}}>
          
                <View style={[styles.plusMinusButton, {backgroundColor: "#90e665"}]}>
                  <ThemedText style={[styles.plusMinusText, {paddingTop: 13.5}]}>💊</ThemedText>
                </View>
    
                <View style={[styles.plusMinusButton, {backgroundColor: "#90e665"}]}>
                  <ThemedText style={[styles.plusMinusText, {paddingTop: 15.5}]}>{getFamilyEmoji()}</ThemedText>
                </View>
                  
                <View style={{flexGrow: 1}}>
                  {isOwnedByMe &&
                    <ThemedText style={[styles.plusMinusText, {alignSelf: 'flex-end', paddingTop: 5}]}>👑</ThemedText>
                  }
                  <ThemedText style={{marginRight: 35, textAlign: 'center'}}>{cabinet.medicineCabinetName}</ThemedText>
                  
                </View>
        
              </View>
            </View>
  
          </Pressable>
        )
      }
      const renderCabinetList = (cabinetList: [Cabinet]) => {
        
        if (!cabinetList) return [];
        let renderedCabinets = [];
  
        for (let i = 0; i < cabinetList.length; i++) { // for debugging i == 1 is the condition
            renderedCabinets.push(renderCabinet(cabinetList[i], i == 1));
        }

        setRenderedCabinets(renderedCabinets);
      };

      const sendGetCabinetsRequest = async () => {

        // bug when adding medication and then trying to get all user medications
        try {
          console.log(user.Token)
          const request = {
            method: 'get',
            url: "http://10.0.2.2:5194/user/cabinet",
            headers: {
                "Authorization": "Bearer " + user.Token, 
            },
            data: {
            }
          }

          const response = await axios(request);
    
          console.log("full: " + response.request._response);
          console.log("status: " + response.request.status);
          
          if (response.request.status == 200) {
            myCabinets = JSON.parse(response.request._response);
            renderCabinetList(myCabinets);
            console.log(myCabinets);
            return true;
          } else if (response.request.status == 401) {
            DataHandler.expireSession();
          }
    
        } catch (error) {
          console.error("Error fetching data:", error);
          DataHandler.expireSession();
        }

      }
      sendGetCabinetsRequest();
  })

  return (    
    <View style={{backgroundColor: backgroundColorMain, flex: 1}}>
        <View style={{flex: 1}}>
        {MrPillLogo(0.5)}
            <View style={styles.pagetop}> 
                <ThemedText style={{textAlign: 'center', fontSize: 24, textDecorationLine: 'underline', fontWeight: 'bold', marginTop: 8}}>
                    ארונות תרופות שלי:{"\n"}
                </ThemedText>
                <ParallaxScrollView backgroundColor={backgroundColorLight}>
                    {renderedCabinets.length > 0 && renderedCabinets}
                    {renderedCabinets.length == 0 && <ThemedText style={{color: "#FF0000"}}>אין ארונות תרופות. נא הוסף תחילה ארון.</ThemedText>}
                </ParallaxScrollView>
            </View>
        </View>

        <View style={styles.pagebottom}>
            <View style={styles.row}>
                <AppHomeButton BackgroundColor={backgroundColorLight} BorderColor={borderColor} ButtonContent={strFC("הוסף ארון חדש")} ButtonAction={()=>{router.navigate('/(cabinet)/addcabinet')}}/>
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
