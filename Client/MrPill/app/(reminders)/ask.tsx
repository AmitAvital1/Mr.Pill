import React, { useRef } from 'react';
import { ScrollView, View, Text, Button } from 'react-native';

export default function App() {
  const scrollViewRef = useRef<ScrollView>(null);

  const scrollToPosition = () => {
    scrollViewRef.current?.scrollTo({ y: 200, animated: true });
  };

  return (
    <ScrollView ref={scrollViewRef}>
      <View style={{ height: 150, backgroundColor: 'lightblue' }}>
        <Text>Section 1</Text>
      </View>
      <View style={{ height: 150, backgroundColor: 'lightgreen' }}>
        <Text>Section 2</Text>
      </View>
      <View style={{ height: 150, backgroundColor: 'lightcoral' }}>
        <Text>Section 3</Text>
      </View>
      <Button title="Scroll to Section 3" onPress={scrollToPosition} />
      <View style={{ height: 150, backgroundColor: 'lightpink' }}>
        <Text>Section 4</Text>
      </View>
      <View style={{ height: 150, backgroundColor: 'lightblue' }}>
        <Text>Section 1</Text>
      </View>
      <View style={{ height: 150, backgroundColor: 'lightgreen' }}>
        <Text>Section 2</Text>
      </View>
      <View style={{ height: 150, backgroundColor: 'lightcoral' }}>
        <Text>Section 3</Text>
      </View>
      <Button title="Scroll to Section 3" onPress={scrollToPosition} />
      <View style={{ height: 150, backgroundColor: 'lightpink' }}>
        <Text>Section 4</Text>
      </View>
    </ScrollView>
  );
}
