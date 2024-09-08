import { AppHomeButton } from '@/components/AppHomeButton';
import { MrPillLogo } from '@/components/MrPillLogo';
import ParallaxScrollView from '@/components/ParallaxScrollView';
import { strFC } from '@/components/strFC';
import DataHandler from '@/DataHandler';
import RequestHandler from '@/RequestHandler';
import { router } from 'expo-router';
import React, { useEffect, useRef, useState } from 'react';
import { View, Text, TouchableOpacity, FlatList, StyleSheet, ScrollView, TextInput, Alert } from 'react-native';

const borderColor = "#c1e9ff";
const bgc = "#ffcbcb";

// abstract list react component
const ChooseList = ({ items, selectedItem, onSelect, listStyle, type, scrollRef }: any) => {
    items = items || [];
    const renderItem = ({ item }: any) => (
        <TouchableOpacity
            key={item} 
            style={[
                styles.item,
                selectedItem === item && styles.selectedItem
            ]}
            onPress={() => onSelect(item)}
        >
            <Text style={[
                styles.itemText,
                selectedItem === item && styles.selectedItemText
            ]}>
                {type == "pill" && item.hebrewName}
                {type == "cabinet" && item}
                {type == "time" && item}
            </Text>
        </TouchableOpacity>
    );

    return (
        <>
            {items.length < 1 &&
                <Text style={{textAlign: 'center', fontSize: 28, color: "#FF0000"}}>לא נמצאו {type == "pill" ? "תרופות בארון" : "ארונות"}</Text> 
            }

            {items.length >= 1 &&
            <FlatList
                data={items}
                renderItem={renderItem}
                keyExtractor={(item) => item}
                showsVerticalScrollIndicator={true}
                contentContainerStyle={[styles.listContainer, listStyle]}
                style={styles.list}
                ref={scrollRef}
            />}
        </>
    );
};

const hoursArr = ["00","01","02","03","04","05","06","07","08","09","10","11","12","13","14","15","16","17","18","19","20","21","22","23"];
const minutesArr = ["00", "15", "30", "45"];
const frequenciesArr = ["פעם אחת בלבד", "פעם בשבוע", "פעם ביומיים", "פעם ביום", "כל 12 שעות", "כל 6 שעות", "כל 4 שעות", "כל 3 שעות"];
const intervalsArr = [undefined, "07:00:00", "02:00:00", "01:00:00", "00:12:00", "00:06:00", "00:04:00", "00:03:00"];
const datesArr = ["היום", "מחר", "בעוד יומיים", "בעוד שלושה ימים", "בעוד ארבעה ימים", "בעוד חמישה ימים", "בעוד שישה ימים", "בעוד שבוע"];

function getDateISO(offsetDays?: number) {

    const result = new Date();
    result.setHours(result.getUTCHours() + 3); // offset to GMT+3

    if (offsetDays) {
        result.setDate(result.getDate() + offsetDays);
    }

    return result.toISOString().slice(0, 11);
}

const AddReminderScreen = () => {  

    const parallaxScrollViewRef = useRef<{ scrollToChild: (index: number) => void }>(null);

    const [isRequestSent, setIsRequestSent] = React.useState<boolean>(false);

    const [myCabinets, setMyCabinets] = React.useState<string[]>();
    const [myPills, setMyPills] = React.useState<any>();

    const [selectedDateOffset, setSelectedDateOffset] = React.useState<string>("");
    const [selectedHours, setSelectedHours] = React.useState<string>();
    const [selectedMinutes, setSelectedMinutes] = React.useState<string>();

    const [selectedCabinet, setSelectedCabinet] = useState<string | null>(null);
    const [selectedPill, setSelectedPill] = useState<any>(null);
    const [selectedFrequency, setSelectedFrequency] = React.useState<string>("");

    const [pillsPerAlert, setPillsPerAlert] = React.useState<string>();
    const [userReminderMessage, setUserReminderMessage] = React.useState<string>();

    async function sendAddReminderRequest() {
        
        DataHandler.set('reminder', {
            "ReminderTime": getDateISO(datesArr.indexOf(selectedDateOffset)) + selectedHours + ":" + selectedMinutes + ":00",
            "Message": userReminderMessage || "עליך לקחת את התרופה " + selectedPill.hebrewName,
            "IsRecurring": !(selectedFrequency === "פעם אחת בלבד"),
            "RecurrenceInterval": intervalsArr[frequenciesArr.indexOf(selectedFrequency)],
            "numOfPills": pillsPerAlert,
            "UserMedicationId": selectedPill.id,
        })

        return await RequestHandler.sendRequest("addReminder");
    }

    async function handleButtonPress() {

        if (!pillsPerAlert) {
            setPillsPerAlert("1");
        }
        
        if (!Number.isInteger(Number(pillsPerAlert))) {
            console.log("PILLS PER ALERT: " + pillsPerAlert);
            Alert.alert("טעות בהזנת מינון לנטילה. אנא הכנס מספר תקין");
            parallaxScrollViewRef.current?.scrollToChild(1);
            return;
        }
        
        if (await sendAddReminderRequest()) {
            Alert.alert("תזכורת נוספה בהצלחה!");
            router.dismiss();
        } else {
            Alert.alert("שגיאה ביצירת התזכורת");
        }
    }

    async function selectCabinet(selection: string) {
        setSelectedCabinet(selection);

        const requestType = selection === "הצג הכל" ? "getAllPills" : "getPills";
        DataHandler.setState('medicineCabinetName', selection);
        
        if (await RequestHandler.sendRequest(requestType)) {

            setMyPills(JSON.parse(RequestHandler.getResponse().request._response).medications);

        } else {

            setMyPills([]);
            setSelectedPill(null);

        }

        // scroll to next list
        parallaxScrollViewRef.current?.scrollToChild(0);
    }

    useEffect(()=>{

        if (isRequestSent) return;
        setIsRequestSent(true);

        const fetchCabinets = async () => {
            if (!await RequestHandler.sendRequest("getMyCabinets")) return;
            setMyCabinets(["הצג הכל",  ...JSON.parse(RequestHandler.getResponse().request._response)
            .map((item: { medicineCabinetName: any; }) => item.medicineCabinetName)]);
            
        }
        fetchCabinets();

    },[])

    return (

        <ParallaxScrollView ref={parallaxScrollViewRef} backgroundColor='#ffdae0' headerImage={MrPillLogo(0.5)} headerHeight={120}>
            <View style={styles.selectionTextContainer}>
                <Text style={styles.selectionText}>{selectedCabinet ? "הארון שנבחר:" : "בחר ארון תרופות:"}</Text>
                <ChooseList
                    items={myCabinets}
                    selectedItem={selectedCabinet}
                    onSelect={selectCabinet}
                    listStyle={[styles.innerList, { }]}
                    type={"cabinet"}
                />
            </View>
            
            {selectedCabinet && 
            <View style={styles.selectionTextContainer}>
                <Text style={styles.selectionText}>{selectedPill ? "התרופה שנבחרה:" : "בחר תרופה:"}</Text>
                <ChooseList
                    items={myPills}
                    selectedItem={selectedPill}
                    onSelect={(pill: any) => {
                        setSelectedPill(pill); 
                        parallaxScrollViewRef.current?.scrollToChild(1);
                    }}
                    listStyle={[styles.innerList, { }]}
                    type={"pill"}
                />
            </View>}

            <View style={{minHeight: 100}}>
                <Text style={styles.selectionText}>מינון התרופה בכל נטילה (ביחידות)</Text>
                <TextInput
                    style={styles.input}
                    onChangeText={setPillsPerAlert}
                    value={pillsPerAlert}
                    placeholder="1"
                    keyboardType="numeric"
                    textAlign="right"
                    onEndEditing={()=>{}}
                />
            </View>

            {selectedPill && 
            <View style={styles.selectionTextContainer}>
                <Text style={styles.selectionText}>{selectedDateOffset ? "היום שבחר:" : "בחר יום להתראה ראשונה:"}</Text>
                <ChooseList
                    items={datesArr}
                    selectedItem={selectedDateOffset}
                    onSelect={(date: string) => {
                        setSelectedDateOffset(date); 
                        parallaxScrollViewRef.current?.scrollToChild(3);
                    }}
                    listStyle={[styles.innerList, { }]}
                    type={"time"}
                />
            </View>}

            {selectedPill &&
            <View style={[styles.pageBottomContainer]}>
                <Text style={styles.selectionText}>{selectedHours && selectedMinutes ? "השעה שנבחרה: " + selectedHours + ":" + selectedMinutes : "בחר שעת התראה:"}</Text>
                <View style={{flexDirection: 'row', margin: 5}}>
                <ScrollView nestedScrollEnabled={true} style={{flex: 1,  height: 305}}>
                    <ChooseList
                        items={hoursArr}
                        selectedItem={selectedHours}
                        onSelect={(hour: string)=> {
                            setSelectedHours(hour);
                            if (selectedMinutes) parallaxScrollViewRef.current?.scrollToChild(4);
                        }}
                        listStyle={[styles.innerList, {  }]}
                        type={"time"}
                    />
                </ScrollView>
                <ScrollView nestedScrollEnabled={true} style={{flex: 1, height: 320}}>
                <ChooseList
                    items={minutesArr}
                    selectedItem={selectedMinutes}
                    onSelect={(minute: string)=> {
                        setSelectedMinutes(minute);
                        if (selectedHours) parallaxScrollViewRef.current?.scrollToChild(4);
                }}
                    listStyle={[styles.innerList, {  }]}
                    type={"time"}
                />
                </ScrollView>
                </View> 
            </View>
            }

            {selectedHours && selectedMinutes && 
            <View style={styles.selectionTextContainer}>
                <Text style={styles.selectionText}>{!selectedFrequency ? "בחר תדירות התראה:" : "התדירות הנבחרה:"}</Text>
                <ChooseList
                    items={frequenciesArr}
                    selectedItem={selectedFrequency}
                    onSelect={(frequency: string)=> {
                        setSelectedFrequency(frequency);
                        parallaxScrollViewRef.current?.scrollToChild(5);
                    }}
                    listStyle={[styles.innerList, { }]}
                    type={"time"}
                />
            </View>
            }

            {selectedFrequency &&
            <View style={{minHeight: 100}}>
                <Text style={styles.selectionText}>{!userReminderMessage ? "הכנס הודעה שתוצג בהתראה:" : "הודעה בהתראה:"}</Text>
                <TextInput
                    style={styles.input}
                    onChangeText={setUserReminderMessage}
                    value={userReminderMessage}
                    placeholder="ההתראה שלי לתרופה"
                    keyboardType="default"
                    textAlign="right"
                    onEndEditing={()=>{}}
                />
            </View>
            }

            <AppHomeButton
                ButtonContent={strFC(selectedFrequency ? "הוסף התראה!" : "נא מלא פרטים")} 
                ButtonAction={selectedFrequency ? handleButtonPress : ()=>{}}
                BackgroundColor={selectedFrequency? "rgb(173, 255, 217)": bgc}
            />

        </ParallaxScrollView>

    );
};

const styles = StyleSheet.create({
    container: {
        flex: 1,
        padding: 20,
        backgroundColor: '#f5f5f5',
        gap: 15,
        borderRadius: 15,
    },
    input: {
        backgroundColor: borderColor,
        height: 70,
        margin: 8,
        borderWidth: 2,
        borderColor: "#888",
        padding: 10,
        borderRadius: 15,
        fontSize: 25,
        elevation: 5,
    },
    selectionTextContainer: {
        flex: 1,
        marginBottom: 10,
    },
    sideBySideContainer: {
        flexDirection: 'row',
        maxHeight: 400,
    },
    pageBottomContainer: {
        maxHeight: 400,
        minHeight: 400,
    },
    blockedContainer: {
        flex: 0,
    },
    selectionText: {
        fontSize: 28,
        marginVertical: 2,
        textAlign: 'center',
        fontWeight: 'bold',
    },
    listContainer: {
        borderRadius: 10,
        paddingVertical: 10,
    },
    list: {
        flex: 1,
        marginHorizontal: 5,
        padding: 5,
    },
    innerList: {
        borderRadius: 15,
        backgroundColor: "#c1e9ff",

        elevation: 5,
    },
    item: {
        padding: 15,
        borderBottomWidth: 1,
        borderBottomColor: '#ccc',
        borderRadius: 15,
        marginHorizontal: 10,
    },
    itemText: {
        fontSize: 28,
        textAlign: 'center',
    },
    selectedItem: {
        backgroundColor: '#007bff',
        
    },
    selectedItemText: {
        color: '#fff',
        fontWeight: 'bold',
    },
});

export default AddReminderScreen;
