import React, { useState, useEffect, useCallback } from "react";
import { View, Text, StyleSheet, Image, Pressable, Modal, SafeAreaView, ScrollView } from "react-native";
import ParallaxScrollView from "@/components/ParallaxScrollView";
import * as FileSystem from 'expo-file-system';

import { MrPillLogo } from "@/components/MrPillLogo";
import Ionicons from '@expo/vector-icons/Ionicons';

import DataHandler from "@/DataHandler";
import RequestHandler from "@/RequestHandler";
import { router } from "expo-router";
import { useFocusEffect } from "@react-navigation/native";
import { PopButton } from "@/components/PopButton";

type Pill = {
  id: number;
  englishName: string | null;
  hebrewName: string | null;
  englishDescription: string | null;
  hebrewDescription: string | null;
  validity: string | null;
  userId: number;
  medicationRepoId: number;
  imagePath: string;
  isPrivate: boolean;
  numberOfPills: number;
  shelfLife: number;
  medicineCabinetName: string | null;
  brochurePath: string | null;
}

const MyPills: React.FC = () => {

  const user = DataHandler.getUser();

  const [myPills, setMyPills] = useState<Pill[]>([]);
  const [dropDownItems, setDropDownItems] = useState<String[]>([]);
  const [cabinetName, setCabinetName] = useState<string>(DataHandler.getState("showFromCabinet", true) || "");
  const [dropdownVisible, setDropdownVisible] = useState(false);
  
  const toggleDropdown = () => {
    setDropdownVisible(!dropdownVisible);
  };

  const fetchPills = async () => {
    
    if (await RequestHandler.sendRequest('getAllPills', true, true)) {
      setMyPills(JSON.parse(RequestHandler.getResponse().request._response).medications);
    }
    
  };

  useFocusEffect(
    useCallback(() => {

      fetchPills();

      return () => {
      };
    }, [])
  );
  
  const handleImagePress = (pill: Pill) => {
    DataHandler.set('pill', pill);
    router.push("/(pills)/singlepill");
  };

  const renderPill = (pill: Pill, index: number) => {

    return ( 
      <SafeAreaView key={pill.id} style={[styles.pillContainer, {backgroundColor: index % 2 == 0? "#bfbfbf" : "#d4d4d4",}]}>
        
          <PopButton ButtonAction={() => handleImagePress(pill)} ButtonContent={
            <View style={styles.imageContainer}>
              <Image source={{uri: pill.imagePath}} style={styles.image} resizeMode="center"/>
            </View>
          } />

        <View style={{maxWidth: 50, minWidth: 50, marginLeft: 25}}>
          <Text style={{textAlign: 'center', fontSize: 36, fontWeight: 'bold', color: pill.numberOfPills < 6 ? "red" : (pill.numberOfPills < 11 ? "yellow" : "green")}}>{pill.numberOfPills}</Text>
        </View>

        <View style={{maxWidth: 170, minWidth: 170,}}>
          {/*<Text style={styles.itemText}>{pill.quantity}</Text>*/}
          <Text style={styles.itemText}>{pill.hebrewName}</Text>
          {pill.isPrivate && !(cabinetName==="me") && <Text style={styles.itemText}>אישי</Text>}
        </View>

      </SafeAreaView>
    );
  };
  
  /*
  const renderPillItems = () => {
    
    let pillItems = [];
    for (let i = 0; i < myPills.length; i++) {
      if (cabinetName === "" || cabinetName === myPills[i].medicineCabinetName || (cabinetName === "me" && myPills[i].isPrivate))
        pillItems.push(renderPill(myPills[i], i));
    }
    return pillItems;
    
  };
  */


  const handleMorePillSources = () => {
    if (!dropdownVisible) {
      setDropDownItems([...new Set(myPills.map((pill, index)=> pill.medicineCabinetName))] as String[]);
    }
    toggleDropdown();
  }

  return (
    <ParallaxScrollView headerHeight={130} headerImage={MrPillLogo(0.5)} backgroundColor="#fceeff">

      <View style={styles.lineContainer}>
        <View style={{alignItems: 'center'}}>
          <Text style={styles.text}>הצג לפי ארון</Text>
          <PopButton  ButtonAction={handleMorePillSources} ButtonContent={
            <Ionicons style={{margin: -10000}} name={'list-circle-outline'} size={cabinetName.length > 2 ? 90 : 80} color={cabinetName.length > 2 ? '#000000' : '#777777'}/>
          }/>
        </View>

        <View style={{alignItems: 'center'}}>
          <Text style={styles.text}>אישי</Text>
          <PopButton  ButtonAction={()=>{setCabinetName("me"); setDropdownVisible(false)}} ButtonContent={
            <Ionicons style={{margin: -10000}} name={'person-circle-outline'} size={cabinetName === "me" ? 90 : 80} color={cabinetName === "me" ? '#000000' : '#777777'}/>
          }/>
        </View>
          
        <View style={{alignItems: 'center'}}>
          <Text style={styles.text}>הצג הכל</Text>
          <PopButton  ButtonAction={()=>{setCabinetName(""); setDropdownVisible(false)}} ButtonContent={
            <Ionicons style={{margin: -10000}} name={'people-circle-outline'} size={cabinetName === "" ? 90 : 80} color={cabinetName === "" ? '#000000' : '#777777'}/>
          }/>
        </View>
      </View>

      <View style={{flexDirection: "row", backgroundColor: "#fceeff", justifyContent: "space-between"}}>
        <Text style={styles.text}> </Text>
        <Text style={styles.text}>כמות במלאי</Text>
        <Text style={styles.text}>תרופה</Text>
      </View>
      
      {dropdownVisible && (
          <View style={styles.dropdown}>
              {dropDownItems.map((item: any, index: any) => (
                  <Pressable key={index} style={styles.dropdownItem} onPress={()=>{setCabinetName(item); setDropdownVisible(false)}}>
                      <Text style={{fontSize: 26, fontWeight: 'bold', textAlign: 'center',}}>{item}</Text>
                  </Pressable>
              ))}
          </View>
      )}
      
      <View style={{gap: 5}}>
        {myPills.filter((pill, index)=>(cabinetName === "" || cabinetName === pill.medicineCabinetName || (cabinetName === "me" && pill.isPrivate)))
                .map((pill, index)=>renderPill(pill, index))}
      </View>
  </ParallaxScrollView>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    flexDirection: 'row',
    padding: 16,
    justifyContent: "center",
    alignItems: "center",
    backgroundColor: "#f3aacd",
    borderRadius: 15,
  },
  text: {
    fontSize: 16,
    marginHorizontal: 20,
    fontWeight: 'bold',
  },
  itemContainer: {
    padding: 8,
    borderBottomWidth: 1,
    borderBottomColor: "#cccccc",
  },
  itemText: {
    fontSize: 24,
    fontWeight: 'bold',
    textAlign: 'right',
    marginHorizontal: 25,
  },
  image: {
    alignSelf: "center",
    height: 100,
    width: 100,
  }, 
  lineContainer: {
    minHeight: 120,
    flexGrow: 1,
    flexDirection: 'row',
    padding: 5,
    justifyContent: "center",
    alignItems: "center",
    alignContent: "center",
    backgroundColor: "#ffddaa",
    borderRadius: 15,
    gap: 20,
  },
  pillContainer: {
    flexGrow: 1,
    flexDirection: 'row',
    padding: 0,
    justifyContent: "center",
    alignItems: "center",
    borderRadius: 15,
  },
  imageContainer: {
    margin: 8,
    height: 90,
    width: 90,
    borderRadius: 20,
    borderWidth: 3,
    borderColor: "#747474",
    backgroundColor: "#bdddf3",
    overflow: 'hidden',
  },
  dropdown: {
      flex: 1,
      borderRadius: 5,
      zIndex: 2,
      marginBottom: 50,
  },
  dropdownItem: {
      backgroundColor: "#ffc2c2",
      flex: 1,
      padding: 10,
      elevation: 3,
      borderRadius: 15,
      zIndex: 5,
      minHeight: 50,
      margin: 3,
      
  },
});

export default MyPills;
