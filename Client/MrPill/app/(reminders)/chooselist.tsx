import DataHandler from '@/DataHandler';
import RequestHandler from '@/RequestHandler';
import React, { useEffect, useState } from 'react';
import { View, Text, TouchableOpacity, FlatList, StyleSheet, Dimensions } from 'react-native';

const { height } = Dimensions.get('window');

// Reusable List Component
const ChooseList = ({ items, selectedItem, onSelect, listStyle }: any) => {
    const renderItem = ({ item }: { item: string }) => (
        <TouchableOpacity   
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
                {item}
            </Text>
        </TouchableOpacity>
    );

    return (
        <FlatList
            data={items}
            renderItem={renderItem}
            keyExtractor={(item) => item}
            showsVerticalScrollIndicator={true}
            contentContainerStyle={[styles.listContainer, listStyle]}
            style={styles.list}
        />
    );
};

const ReminderParameters = () => {

    const [isRequestSent, setIsRequestSent] = React.useState<boolean>(false);
    const [myCabinets, setMyCabinets] = React.useState<any>();
    const [myPills, setMyPills] = React.useState<any>();

    const [selectedInterval, setSelectedInterval] = useState<number | null>(null);
    const [selectedCabinet, setSelectedCabinet] = useState<string | null>(null);
    const [selectedPill, setSelectedPill] = useState<string | null>(null);

    const intervals = Array.from({ length: 240 }, (_, i) => i);
    
    async function selectCabinet(selection: any) {
        setSelectedCabinet(selection);
        DataHandler.setState('medicineCabinetName', selection);
        if (await RequestHandler.sendRequest('getPills')) {
            console.log(RequestHandler.getResponse().request._response);
            //setMyPills(RequestHandler.getResponse().request._response)
        }
    }

    useEffect(()=>{

        if (isRequestSent) return;
        setIsRequestSent(true);

        const fetchCabinets = async () => {
            if (!await RequestHandler.sendRequest("getMyCabinets")) return;
            setMyCabinets(JSON.parse(RequestHandler.getResponse().request._response)
            .map((item: { medicineCabinetName: any; }) => item.medicineCabinetName));
        }
        fetchCabinets();

    },[])

    return (
        <View style={styles.container}>
            <View style={styles.selectionTextContainer}>
                <Text style={styles.selectionText}>תדירות ההתראה: {selectedInterval && "פעם ב-" + selectedInterval + " שעות"  || "לא נבחר"}</Text>
                <Text style={styles.selectionText}>מתוך ארון תרופות: {selectedCabinet || 'לא נבחר'}</Text>
                <Text style={styles.selectionText}>תרופה בשם: {selectedPill || 'לא נבחר'}</Text>
            </View>
            <View style={styles.listsContainer}>
                <ChooseList
                    items={intervals}
                    selectedItem={selectedInterval}
                    onSelect={setSelectedInterval}
                    listStyle={{ backgroundColor: '#ffe4e1' }}  // Custom Style
                />

                <ChooseList
                    items={myPills}
                    selectedItem={selectedPill}
                    onSelect={setSelectedPill}
                    listStyle={{ backgroundColor: '#f0e68c' }}  // Custom Style
                />

                <ChooseList
                    items={myCabinets}
                    selectedItem={selectedCabinet}
                    onSelect={selectCabinet}
                    listStyle={{ backgroundColor: '#e6e6fa' }}  // Custom Style
                />
            </View>
        </View>
    );
};

const styles = StyleSheet.create({
    container: {
        flex: 1,
        padding: 20,
        backgroundColor: '#f5f5f5',
    },
    selectionTextContainer: {
        marginBottom: 10,
    },
    selectionText: {
        fontSize: 16,
        marginVertical: 2,
    },
    listsContainer: {
        flexDirection: 'row',
        height: height / 2,  // Half of the screen height
        width: '100%',       // Full screen width
        justifyContent: 'space-between',
        gap: 10
    },
    listContainer: {
        borderRadius: 10,
        paddingVertical: 10,
    },
    list: {
        flex: 1,
        marginHorizontal: 5,
    },
    item: {
        padding: 15,
        borderBottomWidth: 1,
        borderBottomColor: '#ccc',
        borderRadius: 15,
    },
    itemText: {
        fontSize: 16,
    },
    selectedItem: {
        backgroundColor: '#007bff',
    },
    selectedItemText: {
        color: '#fff',
    },
});

export default ReminderParameters;
