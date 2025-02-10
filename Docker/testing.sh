#!/bin/bash

API_URL="http://localhost:5236/api"

echo "Waiting for the API to start..."
while ! curl -s "$API_URL/auth/login" > /dev/null; do
    sleep 2
done
echo "API is online! Starting tests..."

USER_PAYLOAD='{"email": "nubsgpbr@gmail.com", "password": "Test@123"}'
REGISTER_RESPONSE=$(curl -s -X POST "$API_URL/auth/register" -H "Content-Type: application/json" -d "$USER_PAYLOAD")
echo "Registration response: $REGISTER_RESPONSE"

LOGIN_RESPONSE=$(curl -s -X POST "$API_URL/auth/login" -H "Content-Type: application/json" -d "$USER_PAYLOAD")
TOKEN=$(echo $LOGIN_RESPONSE | jq -r '.token')
echo "Token received: $TOKEN"

if [ "$TOKEN" == "null" ] || [ -z "$TOKEN" ]; then
    echo "Login failed. Terminating tests."
    exit 1
fi

EVENT_PAYLOAD='{"title": "Meeting", "description": "Team meeting", "date": "2025-02-01T10:00:00Z"}'
EVENT_RESPONSE=$(curl -s -X POST "$API_URL/events" -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" -d "$EVENT_PAYLOAD")
EVENT_ID=$(echo $EVENT_RESPONSE | jq -r '.id')
echo "Event created: $EVENT_RESPONSE"

if [ "$EVENT_ID" != "null" ] && [ -n "$EVENT_ID" ]; then
    curl -s -X DELETE "$API_URL/events/$EVENT_ID" -H "Authorization: Bearer $TOKEN"
    echo "Event successfully deleted!"
else
    echo "Failed to create event."
fi

REMINDER_PAYLOAD='{"eventId": "'$EVENT_ID'", "reminderDate": "2025-01-31T09:00:00Z"}'
REMINDER_RESPONSE=$(curl -s -X POST "$API_URL/reminders" -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" -d "$REMINDER_PAYLOAD")
REMINDER_ID=$(echo $REMINDER_RESPONSE | jq -r '.id')
echo "Reminder created: $REMINDER_RESPONSE"

if [ "$REMINDER_ID" != "null" ] && [ -n "$REMINDER_ID" ]; then
    curl -s -X DELETE "$API_URL/reminders/$REMINDER_ID" -H "Authorization: Bearer $TOKEN"
    echo "Reminder successfully deleted!"
else
    echo "Failed to create reminder."
fi

echo "All tests have been completed!"
