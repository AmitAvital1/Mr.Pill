import React, { useState, useEffect } from "react";
import { View, Text, StyleSheet, Image, Pressable, Modal, SafeAreaView } from "react-native";
import ParallaxScrollView from "@/components/ParallaxScrollView";
import * as FileSystem from 'expo-file-system';
import { Downloader } from "@/components/Downloader";
import axios from "axios";
import { MrPillLogo } from "@/components/MrPillLogo";
import Ionicons from '@expo/vector-icons/Ionicons';
import { Colors } from '@/constants/Colors';
import { TouchableOpacity } from "react-native";
import DataHandler from "@/DataHandler";


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
}

/*

"message":"Medications retrieved successfully.",
   "userPhoneNumber":529994444,
   "medications":[
      {

      }

*/

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

      const request = {
        method: 'get',
        url: "http://10.0.2.2:5194/user/all/medications",
        headers: { 
          "Content-Type": "application/json",
          "Authorization": "Bearer " + user.Token,
        }, 
        data: {
          
        }
      }

      try {
        const response = await axios(request);
        if (response.request.status == 200) {
          setMyPills(JSON.parse(response.request._response).medications);
        }
        
      } catch (error) {
        console.error("Error fetching cart items:", error);
      }
    };

    fetchPills();

    
  }, []);

  const handleImagePress = (pillId: number) => {
    console.log(pillId);
  };

  const renderPill = (pill: Pill, index: number) => {

    return ( 
      <SafeAreaView key={pill.id} style={[styles.pillContainer, {backgroundColor: index % 2 == 0? "#b3b3b3" : "#d4d4d4",}]}>
        
        <View>
          <Pressable onPress={() => handleImagePress(pill.id)}>
            <View style={styles.imageContainer}>
            <Image source={{uri: pill.imagePath}} style={styles.image} resizeMode="center"/>
            </View>
          </Pressable>
        </View>

        <View style={{maxWidth: 220, minWidth: 220}}>
          {/*<Text style={styles.itemText}>{pill.quantity}</Text>*/}
          <Text style={styles.itemText}>{pill.hebrewName}</Text>
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
    <ParallaxScrollView headerBackgroundColor={{ light: "#fceeff", dark: "rgb(77, 52, 60)" }} headerImage={MrPillLogo()} backgroundColor="#fceeff">
      
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
      <>
      {dropdownVisible && (
          <View style={styles.dropdown}>
              {dropdownItems.map((item, index) => (
                  <Pressable key={index} style={styles.dropdownItem} onPress={()=>{setOwnerId(Number(item)); setDropdownVisible(false)}}>
                      <Text>{item}</Text>
                  </Pressable>
              ))}
          </View>
      )}
      </>

      {renderPillItems(ownerId)}

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
    flex: 1,
    flexDirection: 'row',
    padding: 0,
    justifyContent: "center",
    alignItems: "center",
    backgroundColor: "#ffddaa",
    borderRadius: 15,
  },
  pillContainer: {
    flex: 1,
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
      borderBottomWidth: 1,
      borderBottomColor: '#cccccc',
      zIndex: 3,
  },
});

export default MyPills;
