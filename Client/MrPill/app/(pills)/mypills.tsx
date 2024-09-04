import React, { useState, useEffect } from "react";
import { View, Text, StyleSheet, Image, Pressable, Modal, SafeAreaView } from "react-native";
import ParallaxScrollView from "@/components/ParallaxScrollView";
import * as FileSystem from 'expo-file-system';

import axios from "axios";
import { MrPillLogo } from "@/components/MrPillLogo";
import Ionicons from '@expo/vector-icons/Ionicons';

import DataHandler from "@/DataHandler";
import RequestHandler from "@/RequestHandler";
import { router } from "expo-router";

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
  const [ownerId, setOwnerId] = useState<number>(0);
  const [dropdownVisible, setDropdownVisible] = useState(false);

  // Example dropdown items
  const dropdownItems = [
      '5',
      '6',
      '7',
  ];

  const toggleDropdown = () => {
      setDropdownVisible(!dropdownVisible);
  };

  useEffect(() => {

    const fetchPills = async () => {
      if (await RequestHandler.sendRequest('getAllPills')) {
        setMyPills(JSON.parse(RequestHandler.getResponse().request._response).medications);
      }
    };

    fetchPills();
    
  }, []);
  
  const handleImagePress = (pill: Pill) => {
    DataHandler.set('pill', pill);
    router.navigate("/(pills)/singlepill");
  };

  const renderPill = (pill: Pill, index: number) => {

    return ( 
      <SafeAreaView key={pill.id} style={[styles.pillContainer, {backgroundColor: index % 2 == 0? "#bfbfbf" : "#d4d4d4",}]}>
        
        <View>
          <Pressable onPress={() => handleImagePress(pill)}>
            <View style={styles.imageContainer}>
              <Image source={{uri: pill.imagePath}} style={styles.image} resizeMode="center"/>
            </View>
          </Pressable>
        </View>

        <View style={{maxWidth: 50, minWidth: 50, marginLeft: 25}}>
          <Text style={{textAlign: 'center', fontSize: 36, fontWeight: 'bold'}}>{pill.numberOfPills}</Text>
        </View>

        <View style={{maxWidth: 170, minWidth: 170,}}>
          {/*<Text style={styles.itemText}>{pill.quantity}</Text>*/}
          <Text style={styles.itemText}>{pill.hebrewName}</Text>
          {pill.isPrivate && <Text style={styles.itemText}>אישי</Text>}
        </View>

      </SafeAreaView>
    );
  };

  const renderPillItems = (ownerId: number) => {
  
    let pillItems = [];
    for (let i = 0; i < myPills.length; i++) {
      if (ownerId == 1 || myPills[i].medicationRepoId == ownerId)
        pillItems.push(renderPill(myPills[i], i));
    }
    return pillItems;
  };

  const handleMorePillSources = () => {
    toggleDropdown();
    console.log(FileSystem.documentDirectory)
  }

  return (
    <ParallaxScrollView headerHeight={130} headerBackgroundColor={{ light: "#fceeff", dark: "rgb(77, 52, 60)" }} headerImage={MrPillLogo(0.5)} backgroundColor="#fceeff">
      
      <View style={styles.lineContainer}>

      <View style={{alignItems: 'center'}}>
          <Text style={styles.text}>בחר ארון</Text>
          <Pressable onPress={handleMorePillSources}>
            <Ionicons name={'list-circle-outline'} size={ownerId >= 2 ? 90 : 80} color={ownerId >= 2 ? '#000000' : '#777777'}/>
          </Pressable>
        </View>

        <View style={{alignItems: 'center'}}>
          <Text style={styles.text}>הצג הכל</Text>
          <Pressable onPress={()=>{setOwnerId(1); setDropdownVisible(false)}}>
            <Ionicons name={'people-circle-outline'} size={ownerId == 1 ? 90 : 80} color={ownerId == 1 ? '#000000' : '#777777'}/>
          </Pressable>
        </View>

        <View style={{alignItems: 'center'}}>
          <Text style={styles.text}>אישי</Text>
          <Pressable onPress={()=>{setOwnerId(0); setDropdownVisible(false)}}>
            <Ionicons name={'person-circle-outline'} size={ownerId == 0 ? 90 : 80} color={ownerId == 0 ? '#000000' : '#777777'}/>
          </Pressable>
        </View>

      </View>

      <View style={[styles.lineContainer, {backgroundColor: "#fceeff",gap: 45}]}>
        <Text style={styles.text}>    </Text>
        <Text style={styles.text}>כמות במלאי</Text>
        <Text style={styles.text}>תרופה</Text>
      </View>
      
      {dropdownVisible && (
          <View style={styles.dropdown}>
              {dropdownItems.map((item, index) => (
                  <Pressable key={index} style={styles.dropdownItem} onPress={()=>{setOwnerId(Number(item)); setDropdownVisible(false)}}>
                      <Text>{item}</Text>
                  </Pressable>
              ))}
          </View>
      )}
      

      <View style={{gap: 5}}>
        {renderPillItems(ownerId)}
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
    flexGrow: 1,
    flexDirection: 'row',
    padding: 5,
    justifyContent: "center",
    alignItems: "center",
    backgroundColor: "#ffddaa",
    borderRadius: 15,
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
      position: 'absolute',
      top: 50, 
      width: 100,
      backgroundColor: '#ffffff',
      borderWidth: 1,
      borderColor: '#cccccc',
      borderRadius: 5,
      elevation: 3, 
      zIndex: 2, 
  },
  dropdownItem: {
      padding: 10,
      borderWidth: 1,
      borderColor: '#000',
      borderRadius: 15,
      zIndex: 3,
  },
});

export default MyPills;
