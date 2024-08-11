import React, { useState, useEffect } from "react";
import { View, Text, StyleSheet, Image, Pressable, Modal } from "react-native";
import ParallaxScrollView from "@/components/ParallaxScrollView";
import * as FileSystem from 'expo-file-system';
import { Downloader } from "@/components/Downloader";
import axios from "axios";
import { MrPillLogo } from "@/components/MrPillLogo";
import Ionicons from '@expo/vector-icons/Ionicons';
import { Colors } from '@/constants/Colors';
import { TouchableOpacity } from "react-native";

type Pill = {
  Id: number;
  Quantity: number;
  EnglishName?: string;
  HebrewName?: string;
  EnglishDescription?: string;
  HebrewDescription?: string;
  Validity?: string; // DateTime type in C#
  UserId: number;
  MedicationRepoId: number;
  ImagePath?: string;
  PrivacyStatusDTO?: boolean; // IsPrivate type in C#
};


const MyPills: React.FC = () => {

  // mock user Id data for shared cabinets
  /*
  const userIds = [
    '5',
    '6',
    '7',
  ];
  const jsonData = JSON.stringify(userIds);
  const filePath = FileSystem.documentDirectory + 'userIds.json';
  const saveFile = async () => {
    try {
        await FileSystem.writeAsStringAsync(filePath, jsonData);
        console.log('User IDs saved successfully!');
    } catch (error) {
        console.error('Error saving user IDs:', error);
    }
  };
  saveFile();
  */

  // for downloading image asset
  // Downloader(uri, pill.Id);

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
        method: 'post',
        url: "http://10.0.2.2:5181/Mr-Pill/user/medications",
        headers: { "Content-Type": "application/json" }, 
        data: {
          "UserToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiR2VuZXJhbC1Vc2VyIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiVXNlciIsIlBob25lTnVtYmVyIjoiMDUwMDExMTIyMiIsImV4cCI6MTcyMjk3NDAxNCwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1MjIxL01yX1BpbGxfQXBwIiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDo1MjIxL01yX1BpbGxfQXBwIn0._c4HO-CKGuXaS-h2EzV89X9eYYGqXaZiyqbqyPyGWYw",
          "PrivacyStatus": "AllMedications",
        }
      }

      try {
        const response = await axios(request);
        console.log(response.data);
        setMyPills(response.data);
      } catch (error) {
        console.error("Error fetching cart items:", error);
      }
    };

    fetchPills();

    // mock pill inventory data
    const mockPills = [
      { Id: 1, UserId: 10, MedicationRepoId: 0, HebrewName: "תרופה 1", Quantity: 10 },
      { Id: 2, UserId: 10, MedicationRepoId: 0, HebrewName: "תרופה 2", Quantity: 15 },
      { Id: 3, UserId: 10, MedicationRepoId: 0, HebrewName: "תרופה 3", Quantity: 20 },
      { Id: 4, UserId: 10, MedicationRepoId: 5, HebrewName: "תרופה 4", Quantity: 12 },
      { Id: 5, UserId: 10, MedicationRepoId: 5, HebrewName: "תרופה 5", Quantity: 30 },
      { Id: 6, UserId: 10, MedicationRepoId: 5, HebrewName: "תרופה 6", Quantity: 25 },
      { Id: 7, UserId: 10, MedicationRepoId: 6, HebrewName: "תרופה 7", Quantity: 18 },
      { Id: 8, UserId: 10, MedicationRepoId: 6, HebrewName: "תרופה 8", Quantity: 22 },
      { Id: 9, UserId: 10, MedicationRepoId: 6, HebrewName: "תרופה 9", Quantity: 28 },
      { Id: 11, UserId: 10, MedicationRepoId: 7, HebrewName: "תרופה 10", Quantity: 14 },
      { Id: 12, UserId: 10, MedicationRepoId: 7, HebrewName: "תרופה 11", Quantity: 16 },
      { Id: 13, UserId: 10, MedicationRepoId: 7, HebrewName: "תרופה 12", Quantity: 13 },
      { Id: 14, UserId: 10, MedicationRepoId: 7, HebrewName: "תרופה 13", Quantity: 19 },
      { Id: 15, UserId: 10, MedicationRepoId: 7, HebrewName: "תרופה 14", Quantity: 21 },
      { Id: 16, UserId: 10, MedicationRepoId: 7, HebrewName: "תרופה 15", Quantity: 17 },
      { Id: 17, UserId: 10, MedicationRepoId: 7, HebrewName: "תרופה 16", Quantity: 11 },
    ];
    setMyPills(mockPills);

  }, []);

  const handleImagePress = (pillId: number) => {
    console.log(pillId);
  };

  const renderPill = (pill: Pill) => {

    return ( 
      <View key={pill.Id} style={styles.lineContainer}>
        
        <Pressable onPress={() => handleImagePress(pill.Id)}>
          <View style={styles.imageContainer}>
          <Image source={{uri: FileSystem.documentDirectory + pill.Id.toString() + '.jpg'}} style={styles.image} />
          </View>
        </Pressable>


        <Text style={styles.itemText}>{pill.Quantity}</Text>
        <Text style={styles.itemText}>{pill.HebrewName}</Text>
        

      </View>
    );
  };

  const renderPillItems = (ownerId: number) => {
  
    let pillItems = [];
    for (let i = 0; i < myPills.length; i++) {
      if (ownerId == 1 || myPills[i].MedicationRepoId == ownerId)
        pillItems.push(renderPill(myPills[i]));
    }
    return pillItems;
  };

  const handleMorePillSources = () => {
    toggleDropdown();
    console.log(FileSystem.documentDirectory)
  }

  return (
    <ParallaxScrollView headerBackgroundColor={{ light: "white", dark: "white" }} headerImage={MrPillLogo()}>
      
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
    fontSize: 32,
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
    backgroundColor: "#f3aacd",
    borderRadius: 15,
  },
  imageContainer: {
    margin: 8,
    height: 90,
    width: 90,
    borderRadius: 20,
    borderWidth: 3,
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
