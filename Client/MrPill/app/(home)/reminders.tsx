import React, { useState, useEffect } from "react";
import { View, Text, StyleSheet, Image, Pressable, Modal } from "react-native";
import * as FileSystem from 'expo-file-system';
import { MrPillLogo } from "@/components/MrPillLogo";
import Ionicons from '@expo/vector-icons/Ionicons';
import ParallaxScrollView from "@/components/ParallaxScrollView";

const bgc = "#5deca2";

type Reminder = {
  Id: number,
  PillId: number,
  Hour: number,
  Minute: number,
  OwnerId: number,
};

const handleImagePress = (pillId: number) => {
  console.log(pillId);
};

const renderReminder = (reminder: Reminder) => {

  return ( 
    <View key={reminder.Id} style={styles.lineContainer}>
      
      <Pressable onPress={() => handleImagePress(reminder.Id)}>
        <View style={styles.imageContainer}>
        <Image source={{uri: FileSystem.documentDirectory + reminder.PillId.toString() + '.jpg'}} style={styles.image} />
        </View>
      </Pressable>
      <View>
      <Text style={styles.itemText}>תרופה מספר {reminder.PillId}</Text>
      <Text style={styles.bigItemText}>{reminder.Hour}:{reminder.Minute}{reminder.Minute == 0 ? 0 : ""}</Text>
      </View>

    </View>
  );
};


const RemindersPage: React.FC = () => {
  
  const [myReminders, setMyReminders] = useState<Reminder[]>([]);
  const [ownerId, setOwnerId] = useState<number>(0);
  const [dropdownVisible, setDropdownVisible] = useState(false);


  useEffect(() => {
    const fetchReminders = async () => {
      try {
        //const response = await axios.get<Pill[]>('YOUR_SERVER_ENDPOINT');
        //setMyPills(response.data);
      } catch (error) {
        console.error("Error fetching cart items:", error);
      }
    };

    fetchReminders();

    // mock pill inventory data
    const mockReminders = [
      { Id: 1, PillId: 3, Hour: 7, Minute: 30, OwnerId: 5},
      { Id: 2, PillId: 4, Hour: 7, Minute: 30, OwnerId: 5},
      { Id: 3, PillId: 5, Hour: 8, Minute: 30, OwnerId: 5},
      { Id: 4, PillId: 1, Hour: 8, Minute: 30, OwnerId: 6},
      { Id: 5, PillId: 2, Hour: 12, Minute: 30, OwnerId: 6},
      { Id: 6, PillId: 6, Hour: 12, Minute: 30, OwnerId: 6},
      { Id: 7, PillId: 7, Hour: 16, Minute: 30, OwnerId: 6},
      { Id: 8, PillId: 8, Hour: 16, Minute: 0, OwnerId: 7},
      { Id: 9, PillId: 9, Hour: 20, Minute: 30, OwnerId: 7},
      { Id: 10, PillId: 10, Hour: 20, Minute: 30, OwnerId: 7},
      { Id: 11, PillId: 11, Hour: 21, Minute: 30, OwnerId: 0},
      { Id: 12, PillId: 12, Hour: 21, Minute: 30, OwnerId: 0},
      { Id: 13, PillId: 13, Hour: 22, Minute: 0, OwnerId: 0},
      { Id: 14, PillId: 14, Hour: 22, Minute: 30, OwnerId: 0},
    ];
    setMyReminders(mockReminders);

  }, []);


  const dropdownItems = [
      '5',
      '6',
      '7',
  ];

  const toggleDropdown = () => {
      setDropdownVisible(!dropdownVisible);
  };

  const renderReminderItems = (ownerId: number) => {

    let reminderItems = [];
    for (let i = 0; i < myReminders.length; i++) {
      if (ownerId == 1 || myReminders[i].OwnerId == ownerId)
        reminderItems.push(renderReminder(myReminders[i]));
    }

    return reminderItems;
  };


  return (
    <ParallaxScrollView headerBackgroundColor={{ light: bgc, dark: bgc }} headerImage={MrPillLogo()} backgroundColor={bgc}>
      <View style={styles.lineContainer}>
      
      <View style={{alignItems: 'center'}}>
          <Text style={styles.text}>בחר מקור</Text>
          <Pressable onPress={() => {}}>
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

      {renderReminderItems(ownerId)}
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
    backgroundColor: "#f2fdbb",
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
    fontSize: 20,
    fontWeight: 'bold',
    textAlign: 'right',
    marginHorizontal: 30,
  },
  bigItemText: {
    fontSize: 50,
    fontWeight: 'bold',
    textAlign: 'right',
    marginHorizontal: 30,
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
    backgroundColor: "#f2fdbb",
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

export default RemindersPage;
