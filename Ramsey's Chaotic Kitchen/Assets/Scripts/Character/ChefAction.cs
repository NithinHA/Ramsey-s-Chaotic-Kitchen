using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class ChefAction : CharacterAction
{
    List<string> keywords_list = new List<string>();
    Dictionary<Transform, Transform> place_transforms = new Dictionary<Transform, Transform>();
    Dictionary<Item, Transform> item_transforms = new Dictionary<Item, Transform>();

    [SerializeField] private GameObject countdown_display_prefab;
    [SerializeField] private Vector3 countdown_display_position_offset;

    public List<InteractableItem> cuttingBoards;
    public List<InteractableItem> cookers;
    public InteractableItem washBasins;
    public InteractableItem servingTable;

    void Start()
    {
        inventory = Inventory.Instance;             // get the Singleton instance of Inventory Class
        keywords_data = KeywordsData.Instance;      // get the Singleton instance of KeywordsData Class
        characterType = CharacterType.chef;

        keywords_list = keywords_data.chef_keywords_2;
        item_transforms = keywords_data.game_item_positions;

        foreach (string keyword in keywords_list)
        {
            string[] word_list = keyword.Split();
            keywords_dict.Add(keyword, () => ActionSelection(word_list));
        }
        // THERE IS SOME PROBLEM HERE! I used to get null reference exception until I Debug.Log'ed the contents of the dictionary, after which the errors simply disappeared.. NANI?

        keyword_recognizer = new KeywordRecognizer(keywords_dict.Keys.ToArray(), ConfidenceLevel.Low);
        keyword_recognizer.OnPhraseRecognized += OnKeywordsRecognized;
    }

    public override void Init(Player ch)
    {
        if (Input.GetKey(KeyCode.Q))
        {
            base.Init(ch);
            place_transforms = ch.GetComponent<ChefData>().chef_interactable_positions;
        }
    }

    protected override void ActionSelection(string[] word_list)
    {
        base.ActionSelection(word_list);

        switch (word_list[0])
        {
            case "chop":
                StartCoroutine(chopping(word_list));
                break;
            case "boil":
                StartCoroutine(boiling(word_list));
                break;
            case "turn":
                StartCoroutine(turn_off(word_list));
                break;
            case "get":
                StartCoroutine(getting_supplies(word_list));
                break;
            case "wash":
                StartCoroutine(washing(word_list));
                break;
            case "prepare":
                StartCoroutine(preparing(word_list));
                break;
            default:
                default_method();
                return;
        }
    }

    IEnumerator chopping(string[] word_list)
    {
        Player playerCopy = character;          // A copy of character is made for use within the coroutine since, the global variable character might change before this coroutine ends.
        string cuttingBoardName = word_list[word_list.Length - 1];
        GameObject cuttingBoardGO = cuttingBoards.Find(item => item.itemKeyword == cuttingBoardName.ToLower()).itemGO;
        if (!playerCopy.is_busy && !cuttingBoardGO.GetComponent<CuttingBoard>().is_chopping)
        {
            playerCopy.PlayVoiceOver(VoiceOverTypes.ActionPositive);
            string item_name = word_list[1];                     // received item to be chopped vegetables/fruits/meat
            Item food_item = keywords_data.findItemWithRawMaterial("chopped " + item_name);
            Transform[] all_transforms = {
                item_transforms[food_item],
                place_transforms[cuttingBoardGO.transform],
                playerCopy.starting_transform
            };        // array of positions where character needs to go

            CuttingBoard cb = cuttingBoardGO.GetComponent<CuttingBoard>();
            playerCopy.is_busy = true;          // set character.is_busy true
            cb.is_chopping = true;      // set cutting_board.is_chopping true

            Animator anim = playerCopy.GetComponent<Animator>();

            //move player to fetch item
            anim.SetBool("is_walking", true);       // play walking anim
            playerCopy.target = all_transforms[0].position;
            playerCopy.target_reached = false;
            while (!playerCopy.target_reached)
                yield return null;
            anim.SetBool("is_walking", false);      // stop walking anim
            //resolve rotations
            StartCoroutine(playerCopy.invokeResolveRotation(all_transforms[0], 1));
            //play get_supplies sound
            AudioManager.Instance?.PlaySound(Constants.Audio.GetSupplies);
            anim.SetTrigger("is_serving");      // get supplies anim
            //wait player for 1s
            yield return new WaitForSeconds(1);

            //move player to chopping board
            anim.SetBool("is_walking", true);       // play walking anim
            playerCopy.target = all_transforms[1].position;
            playerCopy.target_reached = false;
            while (!playerCopy.target_reached)
                yield return null;
            anim.SetBool("is_walking", false);      // stop walking anim
            //resolve rotations
            Coroutine resolve_rotations_cor = StartCoroutine(playerCopy.resolveRotations(all_transforms[1].eulerAngles));
            //call cb.chop()
            cb.chop(food_item);
            //wait player for chopping_delay seconds
            anim.SetBool("is_working", true);       // play working anim
            while (cb.is_chopping)
                yield return null;
            anim.SetBool("is_working", false);      // stop working anim
            //stop resolve_rotations_coroutine
            if (resolve_rotations_cor != null)
                StopCoroutine(resolve_rotations_cor);

            //move player to starting position
            anim.SetBool("is_walking", true);       // play walking anim
            playerCopy.target = all_transforms[2].position;
            playerCopy.target_reached = false;
            while (!playerCopy.target_reached)
                yield return null;
            anim.SetBool("is_walking", false);      // stop walking anim
            //resolve rotations
            StartCoroutine(playerCopy.invokeResolveRotation(all_transforms[2], 1));
            yield return new WaitForSeconds(1);

            //set chef not busy
            playerCopy.is_busy = false;
        }
        else
            playerCopy.PlayVoiceOver(VoiceOverTypes.ActionNegative);
    }

    IEnumerator boiling(string[] word_list)
    {
        Player playerCopy = character;          // A copy of character is made for use within the coroutine since, the global variable character might change before this coroutine ends.
        string cooker_name = word_list[word_list.Length - 1];
        GameObject cookerGO = cookers.Find(item => item.itemKeyword == cooker_name.ToLower()).itemGO;
        if (!playerCopy.is_busy && !cookerGO.GetComponent<Cooker>().is_cooking)      // if character is free and cooker is not cooking anything
        {
            playerCopy.PlayVoiceOver(VoiceOverTypes.ActionPositive);
            string item_name = word_list[1];                     //received item to be cooked rice/noodles
            Item food_item = keywords_data.findItemWithRawMaterial("boiled " + item_name);
            Transform[] all_transforms = { item_transforms[food_item], place_transforms[cookerGO.transform], playerCopy.starting_transform };        //array of positions where character needs to go

            Cooker co = cookerGO.GetComponent<Cooker>();
            playerCopy.is_busy = true;          // set character.is_busy true
            co.is_cooking = true;       // set cooker.is_cooking true

            Animator anim = playerCopy.GetComponent<Animator>();

            //move player to fetch item
            anim.SetBool("is_walking", true);       // play walking anim
            playerCopy.target = all_transforms[0].position;
            playerCopy.target_reached = false;
            while (!playerCopy.target_reached)
                yield return null;
            anim.SetBool("is_walking", false);      // stop walking anim
            //resolve rotations
            StartCoroutine(playerCopy.invokeResolveRotation(all_transforms[0], 1));
            //play get_supplies sound
            AudioManager.Instance?.PlaySound(Constants.Audio.GetSupplies);
            anim.SetTrigger("is_serving");      // get supplies anim
            //wait player for 1s
            yield return new WaitForSeconds(1);

            //move player to cooker
            anim.SetBool("is_walking", true);       // play walking anim
            playerCopy.target = all_transforms[1].position;
            playerCopy.target_reached = false;
            while (!playerCopy.target_reached)
                yield return null;
            anim.SetBool("is_walking", false);      // stop walking anim
            //resolve rotations
            StartCoroutine(playerCopy.invokeResolveRotation(all_transforms[1], 1));
            //call co.cook()
            co.cook(food_item);
            anim.SetTrigger("is_serving");      // turn on cooker anim
            //DO NOT wait player for cooking_delay seconds. Instead, wait player at cooker for 1s
            yield return new WaitForSeconds(1);

            //move player to starting position
            anim.SetBool("is_walking", true);       // play walking anim
            playerCopy.target = all_transforms[2].position;
            playerCopy.target_reached = false;
            while (!playerCopy.target_reached)
                yield return null;
            anim.SetBool("is_walking", false);      // stop walking anim
            //resolve rotations
            StartCoroutine(playerCopy.invokeResolveRotation(all_transforms[2], 1));
            yield return new WaitForSeconds(1);

            //set chef not busy
            playerCopy.is_busy = false;
        }
        else
            playerCopy.PlayVoiceOver(VoiceOverTypes.ActionNegative);
    }

    IEnumerator turn_off(string[] word_list)
    {
        Player playerCopy = character;          // A copy of character is made for use within the coroutine since, the global variable character might change before this coroutine ends.
        string cooker_name = word_list[word_list.Length - 1];
        GameObject cookerGO = cookers.Find(item => item.itemKeyword == cooker_name.ToLower()).itemGO;
        if (!playerCopy.is_busy && cookerGO.GetComponent<Cooker>().is_cooking)       // if character is free and cooker is cooking something
        {
            playerCopy.PlayVoiceOver(VoiceOverTypes.ActionPositive);
            Transform[] all_transforms = { place_transforms[cookerGO.transform], playerCopy.starting_transform };        //array of positions where character needs to go

            Cooker co = cookerGO.GetComponent<Cooker>();
            playerCopy.is_busy = true;          // set character.is_busy true, and cooker.is_cooking is already true

            Animator anim = playerCopy.GetComponent<Animator>();

            //move player to cooker
            anim.SetBool("is_walking", true);       // play walking anim
            playerCopy.target = all_transforms[0].position;
            playerCopy.target_reached = false;
            while (!playerCopy.target_reached)
                yield return null;
            anim.SetBool("is_walking", false);      // stop walking anim
            //resolve rotations
            StartCoroutine(playerCopy.invokeResolveRotation(all_transforms[0], .5f));
            //call co.turn_off_cooker()
            co.turn_off_cooker();
            anim.SetTrigger("is_serving");      // turn off cooker anim
            yield return new WaitForSeconds(.5f);       // player takes 0.5s to turn off cooker

            //move player to starting position
            anim.SetBool("is_walking", true);       // play walking anim
            playerCopy.target = all_transforms[1].position;
            playerCopy.target_reached = false;
            while (!playerCopy.target_reached)
                yield return null;
            anim.SetBool("is_walking", false);      // stop walking anim
            //resolve rotations
            StartCoroutine(playerCopy.invokeResolveRotation(all_transforms[1], 1));
            yield return new WaitForSeconds(1);

            //set chef not busy
            playerCopy.is_busy = false;
        }
        else
            playerCopy.PlayVoiceOver(VoiceOverTypes.ActionNegative);
    }

    IEnumerator getting_supplies(string[] word_list)
    {
        Player playerCopy = character;          // A copy of character is made for use within the coroutine since, the global variable character might change before this coroutine ends.
        if (!playerCopy.is_busy)
        {
            playerCopy.PlayVoiceOver(VoiceOverTypes.ActionPositive);
            StringBuilder sb = new StringBuilder();
            for (int i = 1; i < word_list.Length; i++)
            {
                sb.Append(" " + word_list[i]);          // for items such as "sea weed", item name consist of 2 words. Therefore, we have to use StringBuilder to set item name of any size
            }
            string item_name = sb.ToString().Trim();         // item to be get buns/sea weed
            Item food_item = keywords_data.findItemWithRawMaterial(item_name);
            Transform[] all_transforms = { item_transforms[food_item], playerCopy.starting_transform };        // array of positions where character needs to go

            playerCopy.is_busy = true;          // set character.is_busy true

            Animator anim = playerCopy.GetComponent<Animator>();

            //move player to fetch item
            anim.SetBool("is_walking", true);       // play walking anim
            playerCopy.target = all_transforms[0].position;
            playerCopy.target_reached = false;
            while (!playerCopy.target_reached)
                yield return null;
            anim.SetBool("is_walking", false);      // stop walking anim
            //resolve rotations
            StartCoroutine(playerCopy.invokeResolveRotation(all_transforms[0], food_item.time_to_prepare));
            //play get_supplies sound
            AudioManager.Instance?.PlaySound(Constants.Audio.GetSupplies);
            anim.SetTrigger("is_serving");      // get supplies anim
                                                //wait player for time required to fetch the item
            yield return new WaitForSeconds(food_item.time_to_prepare);

            //add item to inventory... DONE
            bool has_added = inventory.addItem(food_item);
            if (has_added)
            {
                Debug.Log(food_item.name + " added to inventory");
            }
            else
            {
                Debug.Log("can not add " + food_item.name + " to inventory");
                InstructionPanel.Instance.DisplayInstruction("can not add " + food_item.name + " to inventory");
            }

            //move player to starting position
            anim.SetBool("is_walking", true);       // play walking anim
            playerCopy.target = all_transforms[1].position;
            playerCopy.target_reached = false;
            while (!playerCopy.target_reached)
                yield return null;
            anim.SetBool("is_walking", false);      // stop walking anim
            //resolve rotations
            StartCoroutine(playerCopy.invokeResolveRotation(all_transforms[1], 1));
            yield return new WaitForSeconds(1);

            //set chef not busy
            playerCopy.is_busy = false;
        }
        else
            playerCopy.PlayVoiceOver(VoiceOverTypes.ActionNegative);
    }

    IEnumerator washing(string[] word_list)
    {
        Player playerCopy = character;          // A copy of character is made for use within the coroutine since, the global variable character might change before this coroutine ends.
        GameObject sinkGO = washBasins.itemGO;
        GameObject servingTableGO = servingTable.itemGO;
        if (!playerCopy.is_busy && !sinkGO.GetComponent<Sink>().is_washing)
        {
            playerCopy.PlayVoiceOver(VoiceOverTypes.ActionPositive);
            string item_name = word_list[1];                     // received item to be washed (dishes)
            Item utensil_item = keywords_data.findItemWithRawMaterial(item_name);
            Transform[] all_transforms = { place_transforms[servingTableGO.transform], place_transforms[sinkGO.transform], playerCopy.starting_transform };        // array of positions where character needs to go

            Sink sink = sinkGO.GetComponent<Sink>();
            playerCopy.is_busy = true;              // set character.is_busy true
            sink.is_washing = true;         // set sink.is_washing true

            Animator anim = playerCopy.GetComponent<Animator>();

            //move player to fetch item
            anim.SetBool("is_walking", true);       // play walking anim
            playerCopy.target = all_transforms[0].position;
            playerCopy.target_reached = false;
            while (!playerCopy.target_reached)
                yield return null;
            anim.SetBool("is_walking", false);      // stop walking anim
            //resolve rotations
            StartCoroutine(playerCopy.invokeResolveRotation(all_transforms[0], .5f));
            //wait player fetching item
            anim.SetTrigger("is_serving");      // fetch utensil anim
            yield return new WaitForSeconds(.5f);

            //move player to sink
            anim.SetBool("is_walking", true);       // play walking anim
            playerCopy.target = all_transforms[1].position;
            playerCopy.target_reached = false;
            while (!playerCopy.target_reached)
                yield return null;
            anim.SetBool("is_walking", false);      // stop walking anim
            //resolve rotations
            Coroutine resolve_rotations_cor = StartCoroutine(playerCopy.resolveRotations(all_transforms[1].eulerAngles));
            //call sink.washUtensils()
            sink.washUtensils(utensil_item);
            anim.SetBool("is_working", true);       // play washing anim
                                                    //wait player for washing_time seconds
            while (sink.is_washing)
                yield return null;
            anim.SetBool("is_working", false);       // stop washing anim
                                                     //stop resolve_rotations_coroutine
            if (resolve_rotations_cor != null)
                StopCoroutine(resolve_rotations_cor);

            //move player to starting position
            anim.SetBool("is_walking", true);       // play walking anim
            playerCopy.target = all_transforms[2].position;
            playerCopy.target_reached = false;
            while (!playerCopy.target_reached)
                yield return null;
            anim.SetBool("is_walking", false);      // stop walking anim
            //resolve rotations
            StartCoroutine(playerCopy.invokeResolveRotation(all_transforms[2], 1));
            yield return new WaitForSeconds(1);

            //set chef not busy
            playerCopy.is_busy = false;
        }
        else
            playerCopy.PlayVoiceOver(VoiceOverTypes.ActionNegative);
    }

    IEnumerator preparing(string[] word_list)
    {
        Player playerCopy = character;          // A copy of character is made for use within the coroutine since, the global variable character might change before this coroutine ends.
        if (!playerCopy.is_busy)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 1; i < word_list.Length; i++)
            {
                sb.Append(" " + word_list[i]);          // for dishes such as "french fries", dish name consist of 2 words. Therefore, we have to use StringBuilder to set dish name of any size
            }
            string dish_name = sb.ToString().Trim();         // dish to prepare eg.- salad,biryani,sushi,etc..
            Item dish_item = keywords_data.findItemWithRawMaterial(dish_name);

            bool are_ingredients_available = inventory.areIngredientsAvailable(dish_item);  // check if raw materials for dish are available in inventory
            if (are_ingredients_available)
            {
                playerCopy.PlayVoiceOver(VoiceOverTypes.ActionPositive);
                Transform[] all_transforms = { playerCopy.preparing_position, playerCopy.starting_transform };
                playerCopy.is_busy = true;          // set character.is_busy true

                Animator anim = playerCopy.GetComponent<Animator>();

                //move player to player's preparing_position
                anim.SetBool("is_walking", true);       // play walking anim
                playerCopy.target = all_transforms[0].position;
                playerCopy.target_reached = false;
                while (!playerCopy.target_reached)
                    yield return null;
                anim.SetBool("is_walking", false);      // stop walking anim

                //resolve rotations
                StartCoroutine(playerCopy.invokeResolveRotation(all_transforms[0], dish_item.time_to_prepare));

                //remove ingredients used to prepare the dish from inventory
                inventory.removeIngredientsUsed(dish_item);

                //wait player for preparing delay
                // displpay the countdown_timer and then delete it after the countdown is over
                GameObject countdown_display = Instantiate(countdown_display_prefab, all_transforms[0].position + countdown_display_position_offset, Quaternion.Euler(new Vector3(45, 0, 0)));
                countdown_display.GetComponentInChildren<CountdownDisplay>().setTimer(dish_item.time_to_prepare);
                Destroy(countdown_display, dish_item.time_to_prepare);

                anim.SetBool("is_working", true);       // play preparing anim
                yield return new WaitForSeconds(dish_item.time_to_prepare);
                anim.SetBool("is_working", false);       // stop preparing anim

                //add item to inventory
                bool has_added = inventory.addItem(dish_item);
                if (has_added)
                {
                    Debug.Log(dish_item.name + " added to inventory");
                }
                else
                {
                    Debug.Log("can not add " + dish_item.name + " to inventory");
                    InstructionPanel.Instance.DisplayInstruction("can not add " + dish_item.name + " to inventory");
                }

                //move player to starting position
                anim.SetBool("is_walking", true);       // play walking anim
                playerCopy.target = all_transforms[1].position;
                playerCopy.target_reached = false;
                while (!playerCopy.target_reached)
                    yield return null;
                anim.SetBool("is_walking", false);      // stop walking anim
                //resolve rotations
                StartCoroutine(playerCopy.invokeResolveRotation(all_transforms[1], 1));
                yield return new WaitForSeconds(1);

                //set chef not busy
                playerCopy.is_busy = false;
            }
            else
                playerCopy.PlayVoiceOver(VoiceOverTypes.ActionNegative);
        }
        else
            playerCopy.PlayVoiceOver(VoiceOverTypes.ActionNegative);
    }

    void default_method()
    {
        Debug.Log("Invalid input command!");
    }
}
